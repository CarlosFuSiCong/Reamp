using Reamp.Application.Members.Dtos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Reamp.Application.Members.Services
{
    public interface IMemberAppService
    {
        Task<List<AgencyMemberDto>> GetAgencyMembersAsync(
            Guid agencyId,
            CancellationToken ct = default);

        Task<AgencyMemberDto> UpdateAgencyMemberRoleAsync(
            Guid agencyId,
            Guid memberId,
            UpdateAgencyMemberRoleDto dto,
            CancellationToken ct = default);

        Task RemoveAgencyMemberAsync(
            Guid agencyId,
            Guid memberId,
            CancellationToken ct = default);

        Task<List<StudioMemberDto>> GetStudioMembersAsync(
            Guid studioId,
            CancellationToken ct = default);

        Task<StudioMemberDto> UpdateStudioMemberRoleAsync(
            Guid studioId,
            Guid memberId,
            UpdateStudioMemberRoleDto dto,
            CancellationToken ct = default);

        Task RemoveStudioMemberAsync(
            Guid studioId,
            Guid memberId,
            CancellationToken ct = default);
    }
}
