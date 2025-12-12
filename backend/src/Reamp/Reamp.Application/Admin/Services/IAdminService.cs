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
    }
}
