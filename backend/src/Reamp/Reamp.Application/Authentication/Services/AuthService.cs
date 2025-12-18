using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Reamp.Application.Authentication.Dtos;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Infrastructure.Identity;

namespace Reamp.Application.Authentication.Services
{
    // Authentication service implementation
    public sealed class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserProfileRepository _userProfileRepo;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IAgentRepository _agentRepo;
        private readonly IStaffRepository _staffRepo;
        private readonly IUnitOfWork _uow;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            IUserProfileRepository userProfileRepo,
            IRefreshTokenRepository refreshTokenRepo,
            IAgentRepository agentRepo,
            IStaffRepository staffRepo,
            IUnitOfWork uow,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _userProfileRepo = userProfileRepo;
            _refreshTokenRepo = refreshTokenRepo;
            _agentRepo = agentRepo;
            _staffRepo = staffRepo;
            _uow = uow;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<TokenResponse> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
        {
            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new InvalidOperationException("Email is already registered");

            // Create Identity user
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = false // Can be set to true for development
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create user: {errors}");
            }

            // Create UserProfile
            var profile = UserProfile.Create(
                applicationUserId: user.Id,
                firstName: dto.FirstName ?? dto.Email.Split('@')[0],
                lastName: dto.LastName ?? string.Empty,
                role: dto.Role
            );

            await _userProfileRepo.AddAsync(profile, ct);
            await _uow.SaveChangesAsync(ct);

            // Generate tokens
            var tokenResponse = _jwtTokenService.GenerateTokenResponse(
                userId: user.Id,
                email: user.Email!,
                role: dto.Role.ToString()
            );

            // Store refresh token
            var refreshToken = RefreshToken.Create(user.Id, tokenResponse.RefreshToken, _jwtSettings.RefreshTokenExpirationDays);
            await _refreshTokenRepo.AddAsync(refreshToken, ct);
            await _uow.SaveChangesAsync(ct);

            return tokenResponse;
        }

        public async Task<TokenResponse> LoginAsync(LoginDto dto, CancellationToken ct = default)
        {
            // Find user by email
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            // Check if user is soft deleted
            if (user.DeletedAtUtc.HasValue)
                throw new UnauthorizedAccessException("Account has been deleted");

            // Validate password
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid email or password");

            // Get user profile to determine role
            var profile = await _userProfileRepo.GetByApplicationUserIdAsync(user.Id, false, true, ct);
            if (profile == null)
                throw new InvalidOperationException("User profile not found");

            // Generate tokens
            var tokenResponse = _jwtTokenService.GenerateTokenResponse(
                userId: user.Id,
                email: user.Email!,
                role: profile.Role.ToString()
            );

            // Store refresh token
            var refreshToken = RefreshToken.Create(user.Id, tokenResponse.RefreshToken, _jwtSettings.RefreshTokenExpirationDays);
            await _refreshTokenRepo.AddAsync(refreshToken, ct);
            await _uow.SaveChangesAsync(ct);

            return tokenResponse;
        }

        public async Task<UserInfoDto?> GetUserInfoAsync(Guid userId, CancellationToken ct = default)
        {
            // Get Identity user
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.DeletedAtUtc.HasValue)
                return null;

            // Get user profile
            var profile = await _userProfileRepo.GetByApplicationUserIdAsync(userId, false, true, ct);
            if (profile == null)
                return null;

            var userInfo = new UserInfoDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                DisplayName = profile.DisplayName,
                Role = profile.Role,
                CreatedAtUtc = profile.CreatedAtUtc,
                UpdatedAtUtc = profile.UpdatedAtUtc
            };

            // Get agency/studio information based on role
            if (profile.Role == Domain.Accounts.Enums.UserRole.Agent)
            {
                // Query Agent entity
                var agent = await _agentRepo.GetByUserProfileIdAsync(profile.Id, ct);
                if (agent != null)
                {
                    userInfo.AgencyId = agent.AgencyId;
                    userInfo.AgencyRole = (int)agent.Role;
                }
            }
            else if (profile.Role == Domain.Accounts.Enums.UserRole.Staff)
            {
                // Query Staff entity
                var staff = await _staffRepo.GetByUserProfileIdAsync(profile.Id, true, ct);
                if (staff != null)
                {
                    userInfo.StudioId = staff.StudioId;
                    userInfo.StudioRole = (int)staff.Role;
                }
            }

            return userInfo;
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto, CancellationToken ct = default)
        {
            // Get user profile
            var profile = await _userProfileRepo.GetByApplicationUserIdAsync(userId, false, false, ct);
            if (profile == null)
                throw new InvalidOperationException("User profile not found");

            // Update profile
            profile.UpdateBasicInfo(dto.FirstName, dto.LastName);

            await _uow.SaveChangesAsync(ct);
        }

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken ct = default)
        {
            // Get Identity user
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.DeletedAtUtc.HasValue)
                throw new InvalidOperationException("User not found");

            // Verify current password
            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, dto.CurrentPassword);
            if (!isCurrentPasswordValid)
                throw new UnauthorizedAccessException("Current password is incorrect");

            // Change password
            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to change password: {errors}");
            }
        }

        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        {
            // Get refresh token from database
            var storedToken = await _refreshTokenRepo.GetByTokenAsync(refreshToken, ct);
            if (storedToken == null)
                throw new UnauthorizedAccessException("Invalid refresh token");

            // Validate token is active
            if (!storedToken.IsActive)
                throw new UnauthorizedAccessException("Refresh token is expired or revoked");

            // Get user
            var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
            if (user == null || user.DeletedAtUtc.HasValue)
                throw new UnauthorizedAccessException("User not found");

            // Get user profile
            var profile = await _userProfileRepo.GetByApplicationUserIdAsync(user.Id, false, true, ct);
            if (profile == null)
                throw new InvalidOperationException("User profile not found");

            // Generate new tokens
            var newTokenResponse = _jwtTokenService.GenerateTokenResponse(
                userId: user.Id,
                email: user.Email!,
                role: profile.Role.ToString()
            );

            // Revoke old refresh token and store new one
            storedToken.Revoke(newTokenResponse.RefreshToken);
            var newRefreshToken = RefreshToken.Create(user.Id, newTokenResponse.RefreshToken, _jwtSettings.RefreshTokenExpirationDays);
            await _refreshTokenRepo.AddAsync(newRefreshToken, ct);
            await _uow.SaveChangesAsync(ct);

            return newTokenResponse;
        }
    }
}

