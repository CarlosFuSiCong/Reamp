using Reamp.Application.Invitations.Dtos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Reamp.Application.Invitations.Services
{
    public interface IInvitationAppService
    {
        Task<InvitationDetailDto> SendAgencyInvitationAsync(
            Guid agencyId,
            SendAgencyInvitationDto dto,
            Guid currentUserId,
            CancellationToken ct = default);

        Task<InvitationDetailDto> SendStudioInvitationAsync(
            Guid studioId,
            SendStudioInvitationDto dto,
            Guid currentUserId,
            CancellationToken ct = default);

        Task<List<InvitationListDto>> GetMyInvitationsAsync(
            string userEmail,
            CancellationToken ct = default);

        Task<List<InvitationDetailDto>> GetAgencyInvitationsAsync(
            Guid agencyId,
            CancellationToken ct = default);

        Task<List<InvitationDetailDto>> GetStudioInvitationsAsync(
            Guid studioId,
            CancellationToken ct = default);

        Task<InvitationDetailDto?> GetInvitationByIdAsync(
            Guid invitationId,
            CancellationToken ct = default);

        Task AcceptInvitationAsync(
            Guid invitationId,
            Guid userId,
            CancellationToken ct = default);

        Task RejectInvitationAsync(
            Guid invitationId,
            string userEmail,
            CancellationToken ct = default);

        Task CancelInvitationAsync(
            Guid invitationId,
            Guid currentUserId,
            CancellationToken ct = default);
    }
}
