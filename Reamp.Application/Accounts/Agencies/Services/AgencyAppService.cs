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

            // Check if new name results in a different slug
            var newSlug = Slug.From(dto.Name);
            if (newSlug.Value != agency.Slug.Value)
            {
                // Only check for conflicts if slug actually changes
                var existingAgency = await _agencyRepository.GetBySlugAsync(newSlug, asNoTracking: true, ct);
                if (existingAgency != null && existingAgency.Id != agencyId)
                    throw new InvalidOperationException($"An agency with name '{dto.Name}' already exists.");
                
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
    }
}

