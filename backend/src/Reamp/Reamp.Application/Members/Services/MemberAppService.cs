using Microsoft.AspNetCore.Identity;
using Reamp.Application.Members.Dtos;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Reamp.Application.Members.Services
{
    public sealed class MemberAppService : IMemberAppService
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IAgencyBranchRepository _branchRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public MemberAppService(
            IAgentRepository agentRepository,
            IStaffRepository staffRepository,
            IUserProfileRepository userProfileRepository,
            IAgencyBranchRepository branchRepository,
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork)
        {
            _agentRepository = agentRepository;
            _staffRepository = staffRepository;
            _userProfileRepository = userProfileRepository;
            _branchRepository = branchRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AgencyMemberDto>> GetAgencyMembersAsync(
            Guid agencyId,
            CancellationToken ct = default)
        {
            var agents = await _agentRepository.GetByAgencyIdAsync(agencyId, ct);
            var result = new List<AgencyMemberDto>();

            foreach (var agent in agents)
            {
                var profile = await _userProfileRepository.GetByIdAsync(agent.UserProfileId, includeDeleted: false, asNoTracking: true, ct);
                if (profile == null) continue;

                var appUser = await _userManager.FindByIdAsync(profile.ApplicationUserId.ToString());

                string? branchName = null;
                if (agent.AgencyBranchId.HasValue)
                {
                    var branch = await _branchRepository.GetByIdAsync(agent.AgencyBranchId.Value, asNoTracking: true, ct);
                    branchName = branch?.Name;
                }

                result.Add(new AgencyMemberDto
                {
                    Id = agent.Id,
                    UserProfileId = agent.UserProfileId,
                    DisplayName = profile.DisplayName,
                    Email = appUser?.Email ?? string.Empty,
                    AvatarAssetId = profile.AvatarAssetId,
                    Role = agent.Role,
                    AgencyBranchId = agent.AgencyBranchId,
                    AgencyBranchName = branchName,
                    JoinedAtUtc = agent.CreatedAtUtc
                });
            }

            return result.OrderByDescending(m => m.Role).ThenBy(m => m.JoinedAtUtc).ToList();
        }

        public async Task<AgencyMemberDto> UpdateAgencyMemberRoleAsync(
            Guid agencyId,
            Guid memberId,
            UpdateAgencyMemberRoleDto dto,
            CancellationToken ct = default)
        {
            var agent = await _agentRepository.GetByIdAsync(memberId, ct);
            if (agent == null || agent.AgencyId != agencyId)
                throw new KeyNotFoundException("Member not found in this agency.");

            agent.ChangeRole(dto.NewRole);

            await _agentRepository.UpdateAsync(agent, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            var profile = await _userProfileRepository.GetByIdAsync(agent.UserProfileId, includeDeleted: false, asNoTracking: true, ct);
            if (profile == null)
                throw new InvalidOperationException("User profile not found for this member.");
            
            var appUser = await _userManager.FindByIdAsync(profile.ApplicationUserId.ToString());

            string? branchName = null;
            if (agent.AgencyBranchId.HasValue)
            {
                var branch = await _branchRepository.GetByIdAsync(agent.AgencyBranchId.Value, asNoTracking: true, ct);
                branchName = branch?.Name;
            }

            return new AgencyMemberDto
            {
                Id = agent.Id,
                UserProfileId = agent.UserProfileId,
                DisplayName = profile.DisplayName,
                Email = appUser?.Email ?? string.Empty,
                AvatarAssetId = profile.AvatarAssetId,
                Role = agent.Role,
                AgencyBranchId = agent.AgencyBranchId,
                AgencyBranchName = branchName,
                JoinedAtUtc = agent.CreatedAtUtc
            };
        }

        public async Task RemoveAgencyMemberAsync(
            Guid agencyId,
            Guid memberId,
            CancellationToken ct = default)
        {
            var agent = await _agentRepository.GetByIdAsync(memberId, ct);
            if (agent == null || agent.AgencyId != agencyId)
                throw new KeyNotFoundException("Member not found in this agency.");

            await _agentRepository.DeleteAsync(agent, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<List<StudioMemberDto>> GetStudioMembersAsync(
            Guid studioId,
            CancellationToken ct = default)
        {
            var staffList = await _staffRepository.ListByStudioAsync(
                studioId,
                new PageRequest(1, 1000),
                null,
                ct);

            var result = new List<StudioMemberDto>();

            foreach (var staff in staffList.Items)
            {
                var profile = await _userProfileRepository.GetByIdAsync(staff.UserProfileId, includeDeleted: false, asNoTracking: true, ct);
                if (profile == null) continue;

                var appUser = await _userManager.FindByIdAsync(profile.ApplicationUserId.ToString());

                result.Add(new StudioMemberDto
                {
                    Id = staff.Id,
                    UserProfileId = staff.UserProfileId,
                    DisplayName = profile.DisplayName,
                    Email = appUser?.Email ?? string.Empty,
                    AvatarAssetId = profile.AvatarAssetId,
                    Role = staff.Role,
                    Skills = staff.Skills,
                    JoinedAtUtc = staff.CreatedAtUtc
                });
            }

            return result.OrderByDescending(m => m.Role).ThenBy(m => m.JoinedAtUtc).ToList();
        }

        public async Task<StudioMemberDto> UpdateStudioMemberRoleAsync(
            Guid studioId,
            Guid memberId,
            UpdateStudioMemberRoleDto dto,
            CancellationToken ct = default)
        {
            var staff = await _staffRepository.GetByIdAsync(memberId, asNoTracking: false, ct);
            if (staff == null || staff.StudioId != studioId)
                throw new KeyNotFoundException("Member not found in this studio.");

            staff.ChangeRole(dto.NewRole);

            // StaffRepository uses EF tracking, no Update method
            await _unitOfWork.SaveChangesAsync(ct);

            var profile = await _userProfileRepository.GetByIdAsync(staff.UserProfileId, includeDeleted: false, asNoTracking: true, ct);
            if (profile == null)
                throw new InvalidOperationException("User profile not found for this member.");
            
            var appUser = await _userManager.FindByIdAsync(profile.ApplicationUserId.ToString());

            return new StudioMemberDto
            {
                Id = staff.Id,
                UserProfileId = staff.UserProfileId,
                DisplayName = profile.DisplayName,
                Email = appUser?.Email ?? string.Empty,
                AvatarAssetId = profile.AvatarAssetId,
                Role = staff.Role,
                Skills = staff.Skills,
                JoinedAtUtc = staff.CreatedAtUtc
            };
        }

        public async Task RemoveStudioMemberAsync(
            Guid studioId,
            Guid memberId,
            CancellationToken ct = default)
        {
            var staff = await _staffRepository.GetByIdAsync(memberId, asNoTracking: false, ct);
            if (staff == null || staff.StudioId != studioId)
                throw new KeyNotFoundException("Member not found in this studio.");

            _staffRepository.Remove(staff);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
