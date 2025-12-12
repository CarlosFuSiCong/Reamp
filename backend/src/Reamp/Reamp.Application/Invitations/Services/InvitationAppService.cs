using Mapster;
using Microsoft.AspNetCore.Identity;
using Reamp.Application.Invitations.Dtos;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Reamp.Application.Invitations.Services
{
    public sealed class InvitationAppService : IInvitationAppService
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IStudioRepository _studioRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IAgencyBranchRepository _branchRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public InvitationAppService(
            IInvitationRepository invitationRepository,
            IAgencyRepository agencyRepository,
            IStudioRepository studioRepository,
            IAgentRepository agentRepository,
            IStaffRepository staffRepository,
            IUserProfileRepository userProfileRepository,
            IAgencyBranchRepository branchRepository,
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork)
        {
            _invitationRepository = invitationRepository;
            _agencyRepository = agencyRepository;
            _studioRepository = studioRepository;
            _agentRepository = agentRepository;
            _staffRepository = staffRepository;
            _userProfileRepository = userProfileRepository;
            _branchRepository = branchRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<InvitationDetailDto> SendAgencyInvitationAsync(
            Guid agencyId,
            SendAgencyInvitationDto dto,
            Guid currentUserId,
            CancellationToken ct = default)
        {
            var agency = await _agencyRepository.GetByIdAsync(agencyId, asNoTracking: true, ct);
            if (agency == null)
                throw new KeyNotFoundException($"Agency with ID {agencyId} not found.");

            if (dto.AgencyBranchId.HasValue)
            {
                var branch = await _branchRepository.GetByIdAsync(dto.AgencyBranchId.Value, asNoTracking: true, ct);
                if (branch == null || branch.AgencyId != agencyId)
                    throw new KeyNotFoundException("Branch not found.");
            }

            var existingInvitation = await _invitationRepository.GetPendingInvitationAsync(
                agencyId,
                dto.InviteeEmail,
                InvitationType.Agency,
                ct);

            if (existingInvitation != null)
                throw new InvalidOperationException("A pending invitation for this email already exists.");

            var invitation = Invitation.CreateAgencyInvitation(
                agencyId,
                dto.InviteeEmail,
                dto.TargetRole,
                currentUserId,
                dto.AgencyBranchId);

            await _invitationRepository.AddAsync(invitation, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return await MapToDetailDtoAsync(invitation, agency.Name, null, ct);
        }

        public async Task<InvitationDetailDto> SendStudioInvitationAsync(
            Guid studioId,
            SendStudioInvitationDto dto,
            Guid currentUserId,
            CancellationToken ct = default)
        {
            var studio = await _studioRepository.GetByIdAsync(studioId, asNoTracking: true, ct);
            if (studio == null)
                throw new KeyNotFoundException($"Studio with ID {studioId} not found.");

            var existingInvitation = await _invitationRepository.GetPendingInvitationAsync(
                studioId,
                dto.InviteeEmail,
                InvitationType.Studio,
                ct);

            if (existingInvitation != null)
                throw new InvalidOperationException("A pending invitation for this email already exists.");

            var invitation = Invitation.CreateStudioInvitation(
                studioId,
                dto.InviteeEmail,
                dto.TargetRole,
                currentUserId);

            await _invitationRepository.AddAsync(invitation, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return await MapToDetailDtoAsync(invitation, studio.Name, null, ct);
        }

        public async Task<List<InvitationListDto>> GetMyInvitationsAsync(
            string userEmail,
            CancellationToken ct = default)
        {
            var invitations = await _invitationRepository.GetPendingByInviteeEmailAsync(userEmail, ct);

            var result = new List<InvitationListDto>();

            foreach (var invitation in invitations)
            {
                string entityName = string.Empty;

                if (invitation.Type == InvitationType.Agency)
                {
                    var agency = await _agencyRepository.GetByIdAsync(invitation.TargetEntityId, asNoTracking: true, ct);
                    entityName = agency?.Name ?? "Unknown Agency";
                }
                else if (invitation.Type == InvitationType.Studio)
                {
                    var studio = await _studioRepository.GetByIdAsync(invitation.TargetEntityId, asNoTracking: true, ct);
                    entityName = studio?.Name ?? "Unknown Studio";
                }

                result.Add(new InvitationListDto
                {
                    Id = invitation.Id,
                    Type = invitation.Type,
                    TargetEntityName = entityName,
                    TargetRoleName = GetRoleName(invitation),
                    Status = invitation.Status,
                    ExpiresAtUtc = invitation.ExpiresAtUtc,
                    CreatedAtUtc = invitation.CreatedAtUtc,
                    IsExpired = invitation.IsExpired()
                });
            }

            return result;
        }

        public async Task<List<InvitationDetailDto>> GetAgencyInvitationsAsync(
            Guid agencyId,
            CancellationToken ct = default)
        {
            var invitations = await _invitationRepository.GetByTargetEntityAsync(agencyId, InvitationType.Agency, ct);
            var agency = await _agencyRepository.GetByIdAsync(agencyId, asNoTracking: true, ct);

            var result = new List<InvitationDetailDto>();

            foreach (var invitation in invitations)
            {
                string? branchName = null;
                if (invitation.TargetBranchId.HasValue)
                {
                    var branch = await _branchRepository.GetByIdAsync(invitation.TargetBranchId.Value, asNoTracking: true, ct);
                    branchName = branch?.Name;
                }

                result.Add(await MapToDetailDtoAsync(invitation, agency?.Name ?? "Unknown", branchName, ct));
            }

            return result;
        }

        public async Task<List<InvitationDetailDto>> GetStudioInvitationsAsync(
            Guid studioId,
            CancellationToken ct = default)
        {
            var invitations = await _invitationRepository.GetByTargetEntityAsync(studioId, InvitationType.Studio, ct);
            var studio = await _studioRepository.GetByIdAsync(studioId, asNoTracking: true, ct);

            var result = new List<InvitationDetailDto>();

            foreach (var invitation in invitations)
            {
                result.Add(await MapToDetailDtoAsync(invitation, studio?.Name ?? "Unknown", null, ct));
            }

            return result;
        }

        public async Task<InvitationDetailDto?> GetInvitationByIdAsync(
            Guid invitationId,
            CancellationToken ct = default)
        {
            var invitation = await _invitationRepository.GetByIdAsync(invitationId, ct);
            if (invitation == null)
                return null;

            string entityName = string.Empty;
            string? branchName = null;

            if (invitation.Type == InvitationType.Agency)
            {
                var agency = await _agencyRepository.GetByIdAsync(invitation.TargetEntityId, asNoTracking: true, ct);
                entityName = agency?.Name ?? "Unknown Agency";

                if (invitation.TargetBranchId.HasValue)
                {
                    var branch = await _branchRepository.GetByIdAsync(invitation.TargetBranchId.Value, asNoTracking: true, ct);
                    branchName = branch?.Name;
                }
            }
            else if (invitation.Type == InvitationType.Studio)
            {
                var studio = await _studioRepository.GetByIdAsync(invitation.TargetEntityId, asNoTracking: true, ct);
                entityName = studio?.Name ?? "Unknown Studio";
            }

            return await MapToDetailDtoAsync(invitation, entityName, branchName, ct);
        }

        public async Task AcceptInvitationAsync(
            Guid invitationId,
            Guid userId,
            CancellationToken ct = default)
        {
            var invitation = await _invitationRepository.GetByIdAsync(invitationId, ct);
            if (invitation == null)
                throw new KeyNotFoundException($"Invitation with ID {invitationId} not found.");

            var userProfile = await _userProfileRepository.GetByIdAsync(userId, asNoTracking: false, includeDeleted: false, ct: ct);
            if (userProfile == null)
                throw new KeyNotFoundException("User profile not found.");

            var appUser = await _userManager.FindByIdAsync(userProfile.ApplicationUserId.ToString());
            if (appUser == null || appUser.Email?.ToLowerInvariant() != invitation.InviteeEmail)
                throw new InvalidOperationException("Invitation email does not match current user.");

            invitation.Accept(userId);

            if (invitation.Type == InvitationType.Agency)
            {
                var existingAgent = await _agentRepository.GetByUserProfileIdAsync(userId, ct);
                if (existingAgent != null)
                    throw new InvalidOperationException("User is already an agent.");

                var agent = new Agent(
                    userId,
                    invitation.TargetEntityId,
                    invitation.GetAgencyRole(),
                    invitation.TargetBranchId);

                await _agentRepository.AddAsync(agent, ct);
            }
            else if (invitation.Type == InvitationType.Studio)
            {
                var existingStaff = await _staffRepository.GetByUserProfileIdAsync(userId, asNoTracking: true, ct);
                if (existingStaff != null)
                    throw new InvalidOperationException("User is already a staff member.");

                var staff = new Staff(
                    userId,
                    invitation.TargetEntityId,
                    invitation.GetStudioRole());

                await _staffRepository.AddAsync(staff, ct);
            }

            await _invitationRepository.UpdateAsync(invitation, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task RejectInvitationAsync(
            Guid invitationId,
            string userEmail,
            CancellationToken ct = default)
        {
            var invitation = await _invitationRepository.GetByIdAsync(invitationId, ct);
            if (invitation == null)
                throw new KeyNotFoundException($"Invitation with ID {invitationId} not found.");

            // Verify the user is the invitee
            var normalizedEmail = userEmail.Trim().ToLowerInvariant();
            if (invitation.InviteeEmail != normalizedEmail)
                throw new UnauthorizedAccessException("You are not authorized to reject this invitation.");

            invitation.Reject();

            await _invitationRepository.UpdateAsync(invitation, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task CancelInvitationAsync(
            Guid invitationId,
            Guid currentUserId,
            CancellationToken ct = default)
        {
            var invitation = await _invitationRepository.GetByIdAsync(invitationId, ct);
            if (invitation == null)
                throw new KeyNotFoundException($"Invitation with ID {invitationId} not found.");

            // Verify the user is the inviter
            if (invitation.InvitedBy != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to cancel this invitation.");

            invitation.Cancel();

            await _invitationRepository.UpdateAsync(invitation, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        private async Task<InvitationDetailDto> MapToDetailDtoAsync(
            Invitation invitation,
            string entityName,
            string? branchName,
            CancellationToken ct)
        {
            var inviter = await _userProfileRepository.GetByIdAsync(invitation.InvitedBy, includeDeleted: false, asNoTracking: true, ct);

            return new InvitationDetailDto
            {
                Id = invitation.Id,
                Type = invitation.Type,
                TargetEntityId = invitation.TargetEntityId,
                TargetEntityName = entityName,
                TargetBranchId = invitation.TargetBranchId,
                TargetBranchName = branchName,
                InviteeEmail = invitation.InviteeEmail,
                TargetRoleValue = invitation.TargetRoleValue,
                TargetRoleName = GetRoleName(invitation),
                Status = invitation.Status,
                InvitedBy = invitation.InvitedBy,
                InvitedByName = inviter?.DisplayName ?? "Unknown",
                ExpiresAtUtc = invitation.ExpiresAtUtc,
                CreatedAtUtc = invitation.CreatedAtUtc,
                RespondedAtUtc = invitation.RespondedAtUtc,
                IsExpired = invitation.IsExpired()
            };
        }

        private string GetRoleName(Invitation invitation)
        {
            if (invitation.Type == InvitationType.Agency)
                return ((AgencyRole)invitation.TargetRoleValue).ToString();
            else if (invitation.Type == InvitationType.Studio)
                return ((StudioRole)invitation.TargetRoleValue).ToString();

            return "Unknown";
        }
    }
}
