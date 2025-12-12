using Reamp.Application.Admin.Dtos;
using Reamp.Domain.Accounts.Enums;

namespace Reamp.Application.Admin.Services
{
    public interface IAdminService
    {
        Task<AdminStatsResponse> GetStatsAsync(CancellationToken ct);
        Task<List<AdminUserDto>> GetUsersAsync(CancellationToken ct);
        Task UpdateUserStatusAsync(Guid userId, UserStatus status, CancellationToken ct);
        Task UpdateUserRoleAsync(Guid userId, UserRole role, CancellationToken ct);
        
        Task<AgencySummaryDto> CreateAgencyAsync(CreateAgencyForAdminDto dto, CancellationToken ct);
        Task<List<AgencySummaryDto>> GetAgenciesAsync(CancellationToken ct);
        
        Task<StudioSummaryDto> CreateStudioAsync(CreateStudioForAdminDto dto, CancellationToken ct);
        Task<List<StudioSummaryDto>> GetStudiosAsync(CancellationToken ct);
    }
}
