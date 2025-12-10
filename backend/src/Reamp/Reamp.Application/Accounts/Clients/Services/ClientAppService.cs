using Mapster;
using Reamp.Application.Accounts.Clients.Dtos;
using Reamp.Application.Common.Services;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;

namespace Reamp.Application.Accounts.Clients.Services
{
    public sealed class ClientAppService : IClientAppService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IAgencyBranchRepository _branchRepository;
        private readonly IAccountQueryService _queryService;
        private readonly IUnitOfWork _unitOfWork;

        public ClientAppService(
            IClientRepository clientRepository,
            IUserProfileRepository userProfileRepository,
            IAgencyRepository agencyRepository,
            IAgencyBranchRepository branchRepository,
            IAccountQueryService queryService,
            IUnitOfWork unitOfWork)
        {
            _clientRepository = clientRepository;
            _userProfileRepository = userProfileRepository;
            _agencyRepository = agencyRepository;
            _branchRepository = branchRepository;
            _queryService = queryService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ClientDetailDto> CreateAsync(CreateClientDto dto, CancellationToken ct = default)
        {
            var userProfile = await _userProfileRepository.GetByIdAsync(dto.UserProfileId, asNoTracking: true, ct);
            if (userProfile == null || userProfile.DeletedAtUtc != null)
                throw new KeyNotFoundException($"UserProfile with ID {dto.UserProfileId} not found.");

            var agency = await _agencyRepository.GetByIdAsync(dto.AgencyId, asNoTracking: true, ct);
            if (agency == null || agency.DeletedAtUtc != null)
                throw new KeyNotFoundException($"Agency with ID {dto.AgencyId} not found.");

            if (dto.AgencyBranchId.HasValue)
            {
                var branch = await _branchRepository.GetByIdAndAgencyAsync(
                    dto.AgencyBranchId.Value,
                    dto.AgencyId,
                    asNoTracking: true,
                    ct);
                
                if (branch == null)
                    throw new KeyNotFoundException($"AgencyBranch with ID {dto.AgencyBranchId} not found in Agency {dto.AgencyId}.");
            }

            var existingClient = await _clientRepository.GetByUserProfileIdAsync(dto.UserProfileId, asNoTracking: true, ct);
            if (existingClient != null && existingClient.DeletedAtUtc == null)
                throw new InvalidOperationException($"A client already exists for UserProfile {dto.UserProfileId}.");

            var client = new Domain.Accounts.Entities.Client(dto.UserProfileId, dto.AgencyId, dto.AgencyBranchId);
            await _clientRepository.AddAsync(client, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return await BuildClientDetailDtoAsync(client, ct);
        }

        public async Task<ClientDetailDto> UpdateAsync(Guid clientId, UpdateClientDto dto, CancellationToken ct = default)
        {
            var client = await _clientRepository.GetByIdAsync(clientId, asNoTracking: false, ct);
            if (client == null || client.DeletedAtUtc != null)
                throw new KeyNotFoundException($"Client with ID {clientId} not found.");

            var agency = await _agencyRepository.GetByIdAsync(dto.AgencyId, asNoTracking: true, ct);
            if (agency == null || agency.DeletedAtUtc != null)
                throw new KeyNotFoundException($"Agency with ID {dto.AgencyId} not found.");

            if (dto.AgencyBranchId.HasValue)
            {
                var branch = await _branchRepository.GetByIdAndAgencyAsync(
                    dto.AgencyBranchId.Value,
                    dto.AgencyId,
                    asNoTracking: true,
                    ct);
                
                if (branch == null)
                    throw new KeyNotFoundException($"AgencyBranch with ID {dto.AgencyBranchId} not found in Agency {dto.AgencyId}.");
            }

            client.MoveToAgency(dto.AgencyId, dto.AgencyBranchId);
            await _unitOfWork.SaveChangesAsync(ct);

            return await BuildClientDetailDtoAsync(client, ct);
        }

        public async Task<ClientDetailDto?> GetByIdAsync(Guid clientId, CancellationToken ct = default)
        {
            var client = await _clientRepository.GetByIdAsync(clientId, asNoTracking: true, ct);
            if (client == null || client.DeletedAtUtc != null)
                return null;

            return await BuildClientDetailDtoAsync(client, ct);
        }

        public async Task<ClientDetailDto?> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
        {
            var client = await _clientRepository.GetByUserProfileIdAsync(userProfileId, asNoTracking: true, ct);
            if (client == null || client.DeletedAtUtc != null)
                return null;

            return await BuildClientDetailDtoAsync(client, ct);
        }

        public async Task<IPagedList<ClientListDto>> ListByAgencyAsync(
            Guid agencyId, 
            PageRequest pageRequest, 
            CancellationToken ct = default)
        {
            if (!await _queryService.AgencyExistsAsync(agencyId, ct))
                throw new KeyNotFoundException($"Agency with ID {agencyId} not found.");

            var clients = await _queryService.GetClientsByAgencyAsync(agencyId, ct);
            var totalCount = clients.Count;

            var pagedClients = clients
                .OrderByDescending(c => c.CreatedAtUtc)
                .Skip((pageRequest.Page - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize)
                .ToList();

            var dtos = new List<ClientListDto>();
            foreach (var client in pagedClients)
            {
                var userProfile = await _queryService.GetUserProfileAsync(client.UserProfileId, ct);
                var agency = await _queryService.GetAgencyAsync(client.AgencyId, ct);
                var branch = client.AgencyBranchId.HasValue
                    ? await _queryService.GetAgencyBranchAsync(client.AgencyBranchId.Value, ct)
                    : null;

                dtos.Add(new ClientListDto
                {
                    Id = client.Id,
                    UserProfileId = client.UserProfileId,
                    FirstName = userProfile?.FirstName ?? "",
                    LastName = userProfile?.LastName ?? "",
                    DisplayName = userProfile?.DisplayName ?? "",
                    AgencyId = client.AgencyId,
                    AgencyName = agency?.Name,
                    AgencyBranchId = client.AgencyBranchId,
                    AgencyBranchName = branch?.Name,
                    CreatedAtUtc = client.CreatedAtUtc
                });
            }

            return new PagedList<ClientListDto>(
                dtos,
                totalCount,
                pageRequest.Page,
                pageRequest.PageSize
            );
        }

        public async Task DeleteAsync(Guid clientId, CancellationToken ct = default)
        {
            var client = await _clientRepository.GetByIdAsync(clientId, asNoTracking: false, ct);
            if (client == null || client.DeletedAtUtc != null)
                throw new KeyNotFoundException($"Client with ID {clientId} not found.");

            _clientRepository.Remove(client);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        private async Task<ClientDetailDto> BuildClientDetailDtoAsync(Domain.Accounts.Entities.Client client, CancellationToken ct)
        {
            var detailDto = client.Adapt<ClientDetailDto>();
            var userProfile = await _queryService.GetUserProfileAsync(client.UserProfileId, ct);
            var agency = await _queryService.GetAgencyAsync(client.AgencyId, ct);
            var branch = client.AgencyBranchId.HasValue
                ? await _queryService.GetAgencyBranchAsync(client.AgencyBranchId.Value, ct)
                : null;
            
            detailDto.AgencyName = agency?.Name;
            detailDto.AgencyBranchName = branch?.Name;
            detailDto.FirstName = userProfile?.FirstName ?? "";
            detailDto.LastName = userProfile?.LastName ?? "";
            detailDto.DisplayName = userProfile?.DisplayName ?? "";
            
            return detailDto;
        }
    }
}
