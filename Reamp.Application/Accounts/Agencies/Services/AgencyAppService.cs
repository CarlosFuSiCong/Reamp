using Reamp.Application.Accounts.Agencies.Dtos;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Common.ValueObjects;

namespace Reamp.Application.Accounts.Agencies.Services
{
    public sealed class AgencyAppService : IAgencyAppService
    {
        private readonly IAgencyRepository _agencyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AgencyAppService(
            IAgencyRepository agencyRepository,
            IUnitOfWork unitOfWork)
        {
            _agencyRepository = agencyRepository;
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

            return await MapToDetailDtoAsync(agency);
        }

        public async Task<AgencyDetailDto> UpdateAsync(
            Guid agencyId,
            UpdateAgencyDto dto,
            CancellationToken ct = default)
        {
            var agency = await _agencyRepository.GetByIdAsync(agencyId, asNoTracking: false, ct);
            if (agency == null)
                throw new KeyNotFoundException($"Agency with ID {agencyId} not found.");

            // Check if new name conflicts with existing slug
            if (!agency.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                var newSlug = Slug.From(dto.Name);
                if (await _agencyRepository.ExistsBySlugAsync(newSlug, ct))
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

            return await MapToDetailDtoAsync(agency);
        }

        public async Task<AgencyDetailDto?> GetByIdAsync(Guid agencyId, CancellationToken ct = default)
        {
            var agency = await _agencyRepository.GetByIdAsync(agencyId, asNoTracking: true, ct);
            return agency == null ? null : await MapToDetailDtoAsync(agency);
        }

        public async Task<AgencyDetailDto?> GetBySlugAsync(string slug, CancellationToken ct = default)
        {
            var agencySlug = Slug.From(slug);
            var agency = await _agencyRepository.GetBySlugAsync(agencySlug, asNoTracking: true, ct);
            return agency == null ? null : await MapToDetailDtoAsync(agency);
        }

        public async Task<IPagedList<AgencyListDto>> ListAsync(
            PageRequest pageRequest,
            string? search = null,
            CancellationToken ct = default)
        {
            var pagedAgencies = await _agencyRepository.ListAsync(pageRequest, search, ct);

            var dtos = pagedAgencies.Items.Select(agency => new AgencyListDto
            {
                Id = agency.Id,
                Name = agency.Name,
                Slug = agency.Slug.Value,
                Description = agency.Description,
                LogoAssetId = agency.LogoAssetId,
                LogoUrl = null, // TODO: Map from MediaAsset when needed
                ContactEmail = agency.ContactEmail,
                ContactPhone = agency.ContactPhone,
                BranchCount = agency.Branches.Count,
                CreatedAtUtc = agency.CreatedAtUtc
            }).ToList();

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

        public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default)
        {
            var agencySlug = Slug.From(slug);
            return await _agencyRepository.ExistsBySlugAsync(agencySlug, ct);
        }

        private Task<AgencyDetailDto> MapToDetailDtoAsync(Agency agency)
        {
            return Task.FromResult(new AgencyDetailDto
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
                BranchCount = agency.Branches.Count
            });
        }
    }
}

