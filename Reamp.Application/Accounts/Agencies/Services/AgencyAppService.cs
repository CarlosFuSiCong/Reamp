using Reamp.Application.Accounts.Agencies.Dtos;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Common.ValueObjects;
using Reamp.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Reamp.Application.Accounts.Agencies.Services
{
    public sealed class AgencyAppService : IAgencyAppService
    {
        private readonly IAgencyRepository _agencyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;

        public AgencyAppService(
            IAgencyRepository agencyRepository,
            IUnitOfWork unitOfWork,
            ApplicationDbContext dbContext)
        {
            _agencyRepository = agencyRepository;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public async Task<AgencyDetailDto> CreateAsync(
            CreateAgencyDto dto,
            Guid currentUserId,
            CancellationToken ct = default)
        {
            if (currentUserId == Guid.Empty)
                throw new ArgumentException("CurrentUserId is required.", nameof(currentUserId));

            // Check if slug already exists
            var slug = Slug.From(dto.Name);
            if (await _agencyRepository.ExistsBySlugAsync(slug, ct))
                throw new InvalidOperationException($"An agency with name '{dto.Name}' already exists.");

            var agency = Agency.Create(
                name: dto.Name,
                createdBy: currentUserId,
                contactEmail: dto.ContactEmail,
                contactPhone: dto.ContactPhone,
                description: dto.Description,
                logoAssetId: dto.LogoAssetId
            );

            await _agencyRepository.AddAsync(agency, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return MapToDetailDto(agency, 0); // New agency has no branches yet
        }

        public async Task<AgencyDetailDto> UpdateAsync(
            Guid agencyId,
            UpdateAgencyDto dto,
            CancellationToken ct = default)
        {
            var agency = await _agencyRepository.GetByIdAsync(agencyId, asNoTracking: false, ct);
            if (agency == null)
                throw new KeyNotFoundException($"Agency with ID {agencyId} not found.");

            // Check if name changed (case-sensitive to allow casing updates)
            if (agency.Name != dto.Name.Trim())
            {
                var newSlug = Slug.From(dto.Name);
                
                // Only check for slug conflicts if slug actually changes
                if (newSlug.Value != agency.Slug.Value)
                {
                    var existingAgency = await _agencyRepository.GetBySlugAsync(newSlug, asNoTracking: true, ct);
                    if (existingAgency != null && existingAgency.Id != agencyId)
                        throw new InvalidOperationException($"An agency with name '{dto.Name}' already exists.");
                }
                
                agency.Rename(dto.Name);
            }

            agency.UpdateDescription(dto.Description);
            agency.UpdateContact(dto.ContactEmail, dto.ContactPhone);

            if (dto.LogoAssetId.HasValue && dto.LogoAssetId.Value != Guid.Empty)
                agency.SetLogoAsset(dto.LogoAssetId.Value);
            else if (!dto.LogoAssetId.HasValue)
                agency.ClearLogo();

            await _unitOfWork.SaveChangesAsync(ct);

            // Query the branch count after update
            var branchCount = await _dbContext.Set<AgencyBranch>()
                .CountAsync(b => b.AgencyId == agency.Id && b.DeletedAtUtc == null, ct);

            return MapToDetailDto(agency, branchCount);
        }

        public async Task<AgencyDetailDto?> GetByIdAsync(Guid agencyId, CancellationToken ct = default)
        {
            var result = await _dbContext.Set<Agency>()
                .AsNoTracking()
                .Where(a => a.Id == agencyId && a.DeletedAtUtc == null)
                .Select(a => new
                {
                    Agency = a,
                    BranchCount = a.Branches.Count(b => b.DeletedAtUtc == null)
                })
                .FirstOrDefaultAsync(ct);

            if (result == null)
                return null;

            return MapToDetailDto(result.Agency, result.BranchCount);
        }

        public async Task<AgencyDetailDto?> GetBySlugAsync(string slug, CancellationToken ct = default)
        {
            var agencySlug = Slug.From(slug);
            
            var result = await _dbContext.Set<Agency>()
                .AsNoTracking()
                .Where(a => a.Slug.Value == agencySlug.Value && a.DeletedAtUtc == null)
                .Select(a => new
                {
                    Agency = a,
                    BranchCount = a.Branches.Count(b => b.DeletedAtUtc == null)
                })
                .FirstOrDefaultAsync(ct);

            if (result == null)
                return null;

            return MapToDetailDto(result.Agency, result.BranchCount);
        }

        public async Task<IPagedList<AgencyListDto>> ListAsync(
            PageRequest pageRequest,
            string? search = null,
            CancellationToken ct = default)
        {
            var query = _dbContext.Set<Agency>()
                .AsNoTracking()
                .Where(a => a.DeletedAtUtc == null);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(a => a.Name.ToLower().Contains(s) || a.Slug.Value.Contains(s));
            }

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderBy(a => a.Name)
                .Skip((pageRequest.Page - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize)
                .Select(a => new AgencyListDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Slug = a.Slug.Value,
                    Description = a.Description,
                    LogoAssetId = a.LogoAssetId,
                    LogoUrl = null, // TODO: Map from MediaAsset when needed
                    ContactEmail = a.ContactEmail,
                    ContactPhone = a.ContactPhone,
                    BranchCount = a.Branches.Count(b => b.DeletedAtUtc == null),
                    CreatedAtUtc = a.CreatedAtUtc
                })
                .ToListAsync(ct);

            return new PagedList<AgencyListDto>(
                items,
                totalCount,
                pageRequest.Page,
                pageRequest.PageSize
            );
        }

        public async Task DeleteAsync(Guid agencyId, CancellationToken ct = default)
        {
            var agency = await _agencyRepository.GetByIdAsync(agencyId, asNoTracking: false, ct);
            if (agency == null)
                throw new KeyNotFoundException($"Agency with ID {agencyId} not found.");

            _agencyRepository.Remove(agency);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<AgencyDetailDto> UpdateLogoAsync(Guid agencyId, Guid? logoAssetId, CancellationToken ct = default)
        {
            var agency = await _agencyRepository.GetByIdAsync(agencyId, asNoTracking: false, ct);
            if (agency == null)
                throw new KeyNotFoundException($"Agency with ID {agencyId} not found.");

            if (logoAssetId.HasValue && logoAssetId.Value != Guid.Empty)
                agency.SetLogoAsset(logoAssetId.Value);
            else
                agency.ClearLogo();

            await _unitOfWork.SaveChangesAsync(ct);

            var branchCount = await _dbContext.Set<AgencyBranch>()
                .CountAsync(b => b.AgencyId == agency.Id && b.DeletedAtUtc == null, ct);

            return MapToDetailDto(agency, branchCount);
        }

        public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default)
        {
            var agencySlug = Slug.From(slug);
            return await _agencyRepository.ExistsBySlugAsync(agencySlug, ct);
        }

        private AgencyDetailDto MapToDetailDto(Agency agency, int branchCount)
        {
            return new AgencyDetailDto
            {
                Id = agency.Id,
                Name = agency.Name,
                Slug = agency.Slug.Value,
                Description = agency.Description,
                LogoAssetId = agency.LogoAssetId,
                LogoUrl = null, // TODO: Map from MediaAsset when needed
                ContactEmail = agency.ContactEmail,
                ContactPhone = agency.ContactPhone,
                CreatedBy = agency.CreatedBy,
                CreatedAtUtc = agency.CreatedAtUtc,
                UpdatedAtUtc = agency.UpdatedAtUtc,
                BranchCount = branchCount
            };
        }

        // Branch Management Implementation

        public async Task<AgencyBranchDetailDto> CreateBranchAsync(
            Guid agencyId,
            CreateAgencyBranchDto dto,
            Guid currentUserId,
            CancellationToken ct = default)
        {
            var agency = await _agencyRepository.GetByIdAsync(agencyId, asNoTracking: false, ct);
            if (agency == null)
                throw new KeyNotFoundException($"Agency with ID {agencyId} not found.");

            // Check for slug conflicts before creating
            var newSlug = Slug.From(dto.Name);
            var existingBranch = await _dbContext.Set<AgencyBranch>()
                .FirstOrDefaultAsync(b => b.AgencyId == agencyId && b.Slug == newSlug && b.DeletedAtUtc == null, ct);

            if (existingBranch != null)
                throw new InvalidOperationException($"A branch with name '{dto.Name}' already exists in this agency.");

            var branch = agency.AddBranch(
                name: dto.Name,
                createdBy: currentUserId,
                contactEmail: dto.ContactEmail,
                contactPhone: dto.ContactPhone,
                description: dto.Description,
                address: dto.Address?.ToValueObject()
            );

            await _unitOfWork.SaveChangesAsync(ct);

            return MapToBranchDetailDto(branch);
        }

        public async Task<AgencyBranchDetailDto> UpdateBranchAsync(
            Guid agencyId,
            Guid branchId,
            UpdateAgencyBranchDto dto,
            CancellationToken ct = default)
        {
            // Check if parent agency exists and is not deleted
            var agencyExists = await _dbContext.Set<Agency>()
                .AnyAsync(a => a.Id == agencyId && a.DeletedAtUtc == null, ct);

            if (!agencyExists)
                throw new KeyNotFoundException($"Agency with ID {agencyId} not found.");

            var branch = await _dbContext.Set<AgencyBranch>()
                .FirstOrDefaultAsync(b => b.Id == branchId && b.AgencyId == agencyId && b.DeletedAtUtc == null, ct);

            if (branch == null)
                throw new KeyNotFoundException($"Branch with ID {branchId} not found in Agency {agencyId}.");

            // Check for slug conflicts when renaming (only if name actually changes)
            var newSlug = Slug.From(dto.Name);
            if (newSlug.Value != branch.Slug.Value)
            {
                var existingBranch = await _dbContext.Set<AgencyBranch>()
                    .FirstOrDefaultAsync(b => b.AgencyId == agencyId && b.Slug == newSlug && b.DeletedAtUtc == null, ct);

                if (existingBranch != null && existingBranch.Id != branchId)
                    throw new InvalidOperationException($"A branch with name '{dto.Name}' already exists in this agency.");
            }

            branch.Rename(dto.Name);
            branch.UpdateDescription(dto.Description);
            branch.UpdateContact(dto.ContactEmail, dto.ContactPhone);
            
            if (dto.Address != null)
                branch.UpdateAddress(dto.Address.ToValueObject());

            await _unitOfWork.SaveChangesAsync(ct);

            return MapToBranchDetailDto(branch);
        }

        public async Task<AgencyBranchDetailDto?> GetBranchByIdAsync(
            Guid agencyId,
            Guid branchId,
            CancellationToken ct = default)
        {
            // Check if parent agency exists and is not deleted
            var agencyExists = await _dbContext.Set<Agency>()
                .AnyAsync(a => a.Id == agencyId && a.DeletedAtUtc == null, ct);

            if (!agencyExists)
                return null;

            var branch = await _dbContext.Set<AgencyBranch>()
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == branchId && b.AgencyId == agencyId && b.DeletedAtUtc == null, ct);

            return branch == null ? null : MapToBranchDetailDto(branch);
        }

        public async Task<IReadOnlyList<AgencyBranchDetailDto>> ListBranchesAsync(
            Guid agencyId,
            CancellationToken ct = default)
        {
            // Check if parent agency exists and is not deleted
            var agencyExists = await _dbContext.Set<Agency>()
                .AnyAsync(a => a.Id == agencyId && a.DeletedAtUtc == null, ct);

            if (!agencyExists)
                return new List<AgencyBranchDetailDto>();

            var branches = await _dbContext.Set<AgencyBranch>()
                .AsNoTracking()
                .Where(b => b.AgencyId == agencyId && b.DeletedAtUtc == null)
                .OrderBy(b => b.Name)
                .ToListAsync(ct);

            return branches.Select(MapToBranchDetailDto).ToList();
        }

        public async Task DeleteBranchAsync(
            Guid agencyId,
            Guid branchId,
            CancellationToken ct = default)
        {
            // Check if parent agency exists and is not deleted
            var agencyExists = await _dbContext.Set<Agency>()
                .AnyAsync(a => a.Id == agencyId && a.DeletedAtUtc == null, ct);

            if (!agencyExists)
                throw new KeyNotFoundException($"Agency with ID {agencyId} not found.");

            var branch = await _dbContext.Set<AgencyBranch>()
                .FirstOrDefaultAsync(b => b.Id == branchId && b.AgencyId == agencyId && b.DeletedAtUtc == null, ct);

            if (branch == null)
                throw new KeyNotFoundException($"Branch with ID {branchId} not found in Agency {agencyId}.");

            branch.SoftDelete();
            await _unitOfWork.SaveChangesAsync(ct);
        }

        private AgencyBranchDetailDto MapToBranchDetailDto(AgencyBranch branch)
        {
            return new AgencyBranchDetailDto
            {
                Id = branch.Id,
                AgencyId = branch.AgencyId,
                Name = branch.Name,
                Slug = branch.Slug.Value,
                Description = branch.Description,
                ContactEmail = branch.ContactEmail,
                ContactPhone = branch.ContactPhone,
                Address = branch.Address != null ? new AddressDto
                {
                    Line1 = branch.Address.Line1,
                    Line2 = branch.Address.Line2,
                    City = branch.Address.City,
                    State = branch.Address.State,
                    Postcode = branch.Address.Postcode,
                    Country = branch.Address.Country,
                    Latitude = branch.Address.Latitude,
                    Longitude = branch.Address.Longitude
                } : null,
                CreatedBy = branch.CreatedBy,
                CreatedAtUtc = branch.CreatedAtUtc,
                UpdatedAtUtc = branch.UpdatedAtUtc
            };
        }
    }
}

