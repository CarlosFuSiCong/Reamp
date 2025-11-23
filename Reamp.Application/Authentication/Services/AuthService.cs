using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly IUnitOfWork _uow;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            IUserProfileRepository userProfileRepo,
            IUnitOfWork uow)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _userProfileRepo = userProfileRepo;
            _uow = uow;
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
                firstName: dto.FirstName,
                lastName: dto.LastName,
                role: dto.Role
            );

            await _userProfileRepo.AddAsync(profile, ct);
            await _uow.SaveChangesAsync(ct);

            // Generate token
            var token = _jwtTokenService.GenerateTokenResponse(
                userId: user.Id,
                email: user.Email!,
                role: dto.Role.ToString()
            );

            return token;
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

            // Generate token
            var token = _jwtTokenService.GenerateTokenResponse(
                userId: user.Id,
                email: user.Email!,
                role: profile.Role.ToString()
            );

            return token;
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

            return new UserInfoDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                DisplayName = profile.DisplayName,
                Role = profile.Role.ToString()
            };
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
    }
}

