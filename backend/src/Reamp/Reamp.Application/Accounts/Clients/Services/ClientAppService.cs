using Mapster;
using Microsoft.EntityFrameworkCore;
using Reamp.Application.Accounts.Clients.Dtos;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Infrastructure;

namespace Reamp.Application.Accounts.Clients.Services
{
    public sealed class ClientAppService : IClientAppService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;

        public ClientAppService(
            IClientRepository clientRepository,
            IUserProfileRepository userProfileRepository,
            IAgencyRepository agencyRepository,
            IUnitOfWork unitOfWork,
            ApplicationDbContext dbContext)
        {
            _clientRepository = clientRepository;
            _userProfileRepository = userProfileRepository;
            _agencyRepository = agencyRepository;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public async Task<ClientDetailDto> CreateAsync(CreateClientDto dto, CancellationToken ct = default)
        {
            // Validate UserProfile exists
            var userProfile = await _userProfileRepository.GetByIdAsync(dto.UserProfileId, asNoTracking: true, ct);
            if (userProfile == null || userProfile.DeletedAtUtc != null)
                throw new KeyNotFoundException($"UserProfile with ID {dto.UserProfileId} not found.");

            // Validate Agency exists and is not deleted
            var agency = await _agencyRepository.GetByIdAsync(dto.AgencyId, asNoTracking: true, ct);
            if (agency == null || agency.DeletedAtUtc != null)
                throw new KeyNotFoundException($"Agency with ID {dto.AgencyId} not found.");

            // Validate AgencyBranch if provided
            if (dto.AgencyBranchId.HasValue)
            {
                var branch = await _dbContext.Set<AgencyBranch>()
                    .FirstOrDefaultAsync(b => b.Id == dto.AgencyBranchId.Value && 
                                            b.AgencyId == dto.AgencyId && 
                                            b.DeletedAtUtc == null, ct);
                if (branch == null)
                    throw new KeyNotFoundException($"AgencyBranch with ID {dto.AgencyBranchId} not found in Agency {dto.AgencyId}.");
            }

            // Check if client already exists for this UserProfile
            var existingClient = await _clientRepository.GetByUserProfileIdAsync(dto.UserProfileId, asNoTracking: true, ct);
            if (existingClient != null && existingClient.DeletedAtUtc == null)
                throw new InvalidOperationException($"A client already exists for UserProfile {dto.UserProfileId}.");

            var client = new Client(dto.UserProfileId, dto.AgencyId, dto.AgencyBranchId);
            await _clientRepository.AddAsync(client, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            var detailDto = client.Adapt<ClientDetailDto>();
            var prof = await _userProfileRepository.GetByIdAsync(client.UserProfileId, asNoTracking: true, ct);
            var ag = await _agencyRepository.GetByIdAsync(client.AgencyId, asNoTracking: true, ct);
            AgencyBranch? br = null;
            if (client.AgencyBranchId.HasValue)
            {
                br = await _dbContext.Set<AgencyBranch>().AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == client.AgencyBranchId.Value, ct);
            }
            
            detailDto.AgencyName = ag?.Name;
            detailDto.AgencyBranchName = br?.Name;
            detailDto.FirstName = prof?.FirstName ?? "";
            detailDto.LastName = prof?.LastName ?? "";
            detailDto.DisplayName = prof?.DisplayName ?? "";
            
            return detailDto;
        }

        public async Task<ClientDetailDto> UpdateAsync(Guid clientId, UpdateClientDto dto, CancellationToken ct = default)
        {
            var client = await _clientRepository.GetByIdAsync(clientId, asNoTracking: false, ct);
            if (client == null || client.DeletedAtUtc != null)
                throw new KeyNotFoundException($"Client with ID {clientId} not found.");

            // Validate Agency exists and is not deleted
            var agency = await _agencyRepository.GetByIdAsync(dto.AgencyId, asNoTracking: true, ct);
            if (agency == null || agency.DeletedAtUtc != null)
                throw new KeyNotFoundException($"Agency with ID {dto.AgencyId} not found.");

            // Validate AgencyBranch if provided
            if (dto.AgencyBranchId.HasValue)
            {
                var branch = await _dbContext.Set<AgencyBranch>()
                    .FirstOrDefaultAsync(b => b.Id == dto.AgencyBranchId.Value && 
                                            b.AgencyId == dto.AgencyId && 
                                            b.DeletedAtUtc == null, ct);
                if (branch == null)
                    throw new KeyNotFoundException($"AgencyBranch with ID {dto.AgencyBranchId} not found in Agency {dto.AgencyId}.");
            }

            client.MoveToAgency(dto.AgencyId, dto.AgencyBranchId);
            await _unitOfWork.SaveChangesAsync(ct);

            var detailDto = client.Adapt<ClientDetailDto>();
            var profile = await _userProfileRepository.GetByIdAsync(client.UserProfileId, asNoTracking: true, ct);
            var ag = await _agencyRepository.GetByIdAsync(client.AgencyId, asNoTracking: true, ct);
            AgencyBranch? br = null;
            if (client.AgencyBranchId.HasValue)
            {
                br = await _dbContext.Set<AgencyBranch>().AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == client.AgencyBranchId.Value, ct);
            }
            
            detailDto.AgencyName = ag?.Name;
            detailDto.AgencyBranchName = br?.Name;
            detailDto.FirstName = profile?.FirstName ?? "";
            detailDto.LastName = profile?.LastName ?? "";
            detailDto.DisplayName = profile?.DisplayName ?? "";
            
            return detailDto;
        }

        public async Task<ClientDetailDto?> GetByIdAsync(Guid clientId, CancellationToken ct = default)
        {
            var client = await _clientRepository.GetByIdAsync(clientId, asNoTracking: true, ct);
            if (client == null || client.DeletedAtUtc != null)
                return null;

            var detailDto = client.Adapt<ClientDetailDto>();
            var userProfile = await _userProfileRepository.GetByIdAsync(client.UserProfileId, asNoTracking: true, ct);
            var agency = await _agencyRepository.GetByIdAsync(client.AgencyId, asNoTracking: true, ct);
            AgencyBranch? branch = null;
            if (client.AgencyBranchId.HasValue)
            {
                branch = await _dbContext.Set<AgencyBranch>().AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == client.AgencyBranchId.Value, ct);
            }
            
            detailDto.AgencyName = agency?.Name;
            detailDto.AgencyBranchName = branch?.Name;
            detailDto.FirstName = userProfile?.FirstName ?? "";
            detailDto.LastName = userProfile?.LastName ?? "";
            detailDto.DisplayName = userProfile?.DisplayName ?? "";
            
            return detailDto;
        }

        public async Task<ClientDetailDto?> GetByUserProfileIdAsync(Guid userProfileId, CancellationToken ct = default)
        {
            var client = await _clientRepository.GetByUserProfileIdAsync(userProfileId, asNoTracking: true, ct);
            if (client == null || client.DeletedAtUtc != null)
                return null;

            var detailDto = client.Adapt<ClientDetailDto>();
            var userProfile = await _userProfileRepository.GetByIdAsync(client.UserProfileId, asNoTracking: true, ct);
            var agency = await _agencyRepository.GetByIdAsync(client.AgencyId, asNoTracking: true, ct);
            AgencyBranch? branch = null;
            if (client.AgencyBranchId.HasValue)
            {
                branch = await _dbContext.Set<AgencyBranch>().AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == client.AgencyBranchId.Value, ct);
            }
            
            detailDto.AgencyName = agency?.Name;
            detailDto.AgencyBranchName = branch?.Name;
            detailDto.FirstName = userProfile?.FirstName ?? "";
            detailDto.LastName = userProfile?.LastName ?? "";
            detailDto.DisplayName = userProfile?.DisplayName ?? "";
            
            return detailDto;
        }

        public async Task<IPagedList<ClientListDto>> ListByAgencyAsync(
            Guid agencyId, 
            PageRequest pageRequest, 
            CancellationToken ct = default)
        {
            // Validate Agency exists
            var agencyExists = await _dbContext.Set<Agency>()
                .AnyAsync(a => a.Id == agencyId && a.DeletedAtUtc == null, ct);

            if (!agencyExists)
                throw new KeyNotFoundException($"Agency with ID {agencyId} not found.");

            var query = _dbContext.Set<Client>()
                .AsNoTracking()
                .Where(c => c.AgencyId == agencyId && c.DeletedAtUtc == null);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(c => c.CreatedAtUtc)
                .Skip((pageRequest.Page - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize)
                .Select(c => new
                {
                    Client = c,
                    UserProfile = _dbContext.Set<UserProfile>().FirstOrDefault(u => u.Id == c.UserProfileId),
                    Agency = _dbContext.Set<Agency>().FirstOrDefault(a => a.Id == c.AgencyId),
                    Branch = c.AgencyBranchId.HasValue 
                        ? _dbContext.Set<AgencyBranch>().FirstOrDefault(b => b.Id == c.AgencyBranchId) 
                        : null
                })
                .ToListAsync(ct);

            var dtos = items.Select(item => new ClientListDto
            {
                Id = item.Client.Id,
                UserProfileId = item.Client.UserProfileId,
                FirstName = item.UserProfile?.FirstName ?? "",
                LastName = item.UserProfile?.LastName ?? "",
                DisplayName = item.UserProfile?.DisplayName ?? "",
                AgencyId = item.Client.AgencyId,
                AgencyName = item.Agency?.Name,
                AgencyBranchId = item.Client.AgencyBranchId,
                AgencyBranchName = item.Branch?.Name,
                CreatedAtUtc = item.Client.CreatedAtUtc
            }).ToList();

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
    }
}

