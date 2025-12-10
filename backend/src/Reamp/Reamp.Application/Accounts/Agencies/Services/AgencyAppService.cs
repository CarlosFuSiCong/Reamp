using Mapster;
using Reamp.Application.Accounts.Agencies.Dtos;
using Reamp.Application.Common.Services;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Common.ValueObjects;

namespace Reamp.Application.Accounts.Agencies.Services
{
    public sealed class AgencyAppService : IAgencyAppService
    {
        private readonly IAgencyRepository _agencyRepository;
        private readonly IAgencyBranchRepository _branchRepository;
        private readonly IAccountQueryService _queryService;
        private readonly IUnitOfWork _unitOfWork;

        public AgencyAppService(
            IAgencyRepository agencyRepository,
            IAgencyBranchRepository branchRepository,
            IAccountQueryService queryService,
            IUnitOfWork unitOfWork)
        {
            _agencyRepository = agencyRepository;
            _branchRepository = branchRepository;
            _queryService = queryService;
            _unitOfWork = unitOfWork;
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

            var detailDto = agency.Adapt<AgencyDetailDto>();
            detailDto.BranchCount = 0;
            return detailDto;
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

            var branchCount = await _branchRepository.CountByAgencyAsync(agency.Id, ct);

            var detailDto = agency.Adapt<AgencyDetailDto>();
            detailDto.BranchCount = branchCount;
            return detailDto;
        }

        public async Task<AgencyDetailDto?> GetByIdAsync(Guid agencyId, CancellationToken ct = default)
        {
            var agency = await _agencyRepository.GetByIdAsync(agencyId, asNoTracking: true, ct);
            if (agency == null)
                return null;

            var detailDto = agency.Adapt<AgencyDetailDto>();
            detailDto.BranchCount = await _branchRepository.CountByAgencyAsync(agencyId, ct);
            return detailDto;
        }

        public async Task<AgencyDetailDto?> GetBySlugAsync(string slug, CancellationToken ct = default)
        {
            var agencySlug = Slug.From(slug);
            var agency = await _agencyRepository.GetBySlugAsync(agencySlug, asNoTracking: true, ct);
            
            if (agency == null)
                return null;

            var detailDto = agency.Adapt<AgencyDetailDto>();
            detailDto.BranchCount = await _branchRepository.CountByAgencyAsync(agency.Id, ct);
            return detailDto;
        }

        public async Task<IPagedList<AgencyListDto>> ListAsync(
            PageRequest pageRequest,
            string? search = null,
            CancellationToken ct = default)
        {
            var pagedAgencies = await _agencyRepository.ListAsync(pageRequest, search, ct);
            
            var dtos = new List<AgencyListDto>();
            foreach (var agency in pagedAgencies.Items)
            {
                var branchCount = await _branchRepository.CountByAgencyAsync(agency.Id, ct);
                var dto = agency.Adapt<AgencyListDto>();
                dto.BranchCount = branchCount;
                dtos.Add(dto);
            }

            return new PagedList<AgencyListDto>(
                dtos,
                pagedAgencies.TotalCount,
                pagedAgencies.Page,
                pagedAgencies.PageSize
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

            var branchCount = await _branchRepository.CountByAgencyAsync(agency.Id, ct);

            var detailDto = agency.Adapt<AgencyDetailDto>();
            detailDto.BranchCount = branchCount;
            return detailDto;
        }

        public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default)
        {
            var agencySlug = Slug.From(slug);
            return await _agencyRepository.ExistsBySlugAsync(agencySlug, ct);
        }

        public async Task<AgencyBranchDetailDto> CreateBranchAsync(
            Guid agencyId,
            CreateAgencyBranchDto dto,
            Guid currentUserId,
            CancellationToken ct = default)
        {
            if (currentUserId == Guid.Empty)
                throw new ArgumentException("CurrentUserId is required.", nameof(currentUserId));

            var agency = await _agencyRepository.GetByIdAsync(agencyId, asNoTracking: false, ct);
            if (agency == null || agency.DeletedAtUtc != null)
                throw new KeyNotFoundException($"Agency with ID {agencyId} not found.");

            // Check for slug conflicts before creating
            var newSlug = Slug.From(dto.Name);
            var existingBranch = await _branchRepository.GetBySlugAsync(agencyId, newSlug, asNoTracking: true, ct);

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
            if (!await _queryService.AgencyExistsAsync(agencyId, ct))
                throw new KeyNotFoundException($"Agency with ID {agencyId} not found.");

            var branch = await _branchRepository.GetByIdAndAgencyAsync(branchId, agencyId, asNoTracking: false, ct);

            if (branch == null)
                throw new KeyNotFoundException($"Branch with ID {branchId} not found in Agency {agencyId}.");

            // Check for slug conflicts when renaming (only if name actually changes)
            var newSlug = Slug.From(dto.Name);
            if (newSlug.Value != branch.Slug.Value)
            {
                var existingBranch = await _branchRepository.GetBySlugAsync(agencyId, newSlug, asNoTracking: true, ct);

                if (existingBranch != null && existingBranch.Id != branchId)
                    throw new InvalidOperationException($"A branch with name '{dto.Name}' already exists in this agency.");
            }

            branch.Rename(dto.Name);
            branch.UpdateDescription(dto.Description);
            branch.UpdateContact(dto.ContactEmail, dto.ContactPhone);
            
            // Always update address to allow clearing it with null
            branch.UpdateAddress(dto.Address?.ToValueObject());

            await _unitOfWork.SaveChangesAsync(ct);

            return MapToBranchDetailDto(branch);
        }

        public async Task<AgencyBranchDetailDto?> GetBranchByIdAsync(
            Guid agencyId,
            Guid branchId,
            CancellationToken ct = default)
        {
            if (!await _queryService.AgencyExistsAsync(agencyId, ct))
                return null;

            var branch = await _branchRepository.GetByIdAndAgencyAsync(branchId, agencyId, asNoTracking: true, ct);

            return branch == null ? null : MapToBranchDetailDto(branch);
        }

        public async Task<IReadOnlyList<AgencyBranchDetailDto>> ListBranchesAsync(
            Guid agencyId,
            CancellationToken ct = default)
        {
            if (!await _queryService.AgencyExistsAsync(agencyId, ct))
                throw new KeyNotFoundException($"Agency with ID {agencyId} not found.");

            var branches = await _branchRepository.GetByAgencyAsync(agencyId, asNoTracking: true, ct);

            return branches.Select(MapToBranchDetailDto).ToList();
        }

        public async Task DeleteBranchAsync(
            Guid agencyId,
            Guid branchId,
            CancellationToken ct = default)
        {
            // Check if parent agency exists and is not deleted
            if (!await _queryService.AgencyExistsAsync(agencyId, ct))
                throw new KeyNotFoundException($"Agency with ID {agencyId} not found.");

            var branch = await _branchRepository.GetByIdAndAgencyAsync(branchId, agencyId, asNoTracking: false, ct);

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

