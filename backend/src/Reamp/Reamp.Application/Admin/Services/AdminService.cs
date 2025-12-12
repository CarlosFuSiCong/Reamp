using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reamp.Application.Admin.Dtos;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Listings.Enums;
using Reamp.Domain.Shoots.Enums;
using Reamp.Infrastructure;
using Reamp.Infrastructure.Identity;

namespace Reamp.Application.Admin.Services
{
    public sealed class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserProfileRepository _userProfileRepo;
        private readonly IAgencyRepository _agencyRepo;
        private readonly IStudioRepository _studioRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            ApplicationDbContext dbContext,
            IUserProfileRepository userProfileRepo,
            IAgencyRepository agencyRepo,
            IStudioRepository studioRepo,
            UserManager<ApplicationUser> userManager,
            IUnitOfWork uow,
            ILogger<AdminService> logger)
        {
            _dbContext = dbContext;
            _userProfileRepo = userProfileRepo;
            _agencyRepo = agencyRepo;
            _studioRepo = studioRepo;
            _userManager = userManager;
            _uow = uow;
            _logger = logger;
        }

        public async Task<AdminStatsResponse> GetStatsAsync(CancellationToken ct)
        {
            var totalUsers = await _dbContext.UserProfiles.CountAsync(ct);
            var activeListings = await _dbContext.Listings
                .CountAsync(l => l.Status == ListingStatus.Active, ct);
            var totalOrders = await _dbContext.ShootOrders.CountAsync(ct);
            var totalStudios = await _dbContext.Studios.CountAsync(ct);

            // Get last 7 days data for chart
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7).Date;
            var chartData = new List<ChartDataPoint>();

            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.UtcNow.AddDays(-i).Date;
                var nextDate = date.AddDays(1);

                var ordersCount = await _dbContext.ShootOrders
                    .CountAsync(o => o.CreatedAtUtc >= date && o.CreatedAtUtc < nextDate, ct);
                
                var listingsCount = await _dbContext.Listings
                    .CountAsync(l => l.CreatedAtUtc >= date && l.CreatedAtUtc < nextDate, ct);

                chartData.Add(new ChartDataPoint
                {
                    Date = date.ToString("MM/dd"),
                    Orders = ordersCount,
                    Listings = listingsCount
                });
            }

            // Generate system alerts
            var alerts = new List<SystemAlert>();
            
            var pendingOrders = await _dbContext.ShootOrders
                .CountAsync(o => o.Status == ShootOrderStatus.Placed, ct);
            if (pendingOrders > 0)
            {
                alerts.Add(new SystemAlert
                {
                    Title = "Pending Orders",
                    Message = $"{pendingOrders} order(s) waiting for acceptance"
                });
            }

            var pendingListings = await _dbContext.Listings
                .CountAsync(l => l.Status == ListingStatus.Pending, ct);
            if (pendingListings > 0)
            {
                alerts.Add(new SystemAlert
                {
                    Title = "Pending Listings",
                    Message = $"{pendingListings} listing(s) awaiting approval"
                });
            }

            // Get recent activities
            var recentOrders = await _dbContext.ShootOrders
                .OrderByDescending(o => o.CreatedAtUtc)
                .Take(5)
                .Select(o => new ActivityDto
                {
                    Id = o.Id.ToString(),
                    Type = "order",
                    Description = $"New order placed",
                    Timestamp = o.CreatedAtUtc
                })
                .ToListAsync(ct);

            var recentListings = await _dbContext.Listings
                .OrderByDescending(l => l.CreatedAtUtc)
                .Take(5)
                .Select(l => new ActivityDto
                {
                    Id = l.Id.ToString(),
                    Type = "listing",
                    Description = $"New listing created",
                    Timestamp = l.CreatedAtUtc
                })
                .ToListAsync(ct);

            var activities = recentOrders.Concat(recentListings)
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .ToList();

            var stats = new AdminStatsDto
            {
                TotalUsers = totalUsers,
                ActiveListings = activeListings,
                TotalOrders = totalOrders,
                TotalStudios = totalStudios,
                ChartData = chartData,
                Alerts = alerts
            };

            return new AdminStatsResponse
            {
                Stats = stats,
                Activities = activities
            };
        }

        public async Task<List<AdminUserDto>> GetUsersAsync(CancellationToken ct)
        {
            var profiles = await _dbContext.UserProfiles
                .OrderByDescending(u => u.CreatedAtUtc)
                .ToListAsync(ct);

            var users = new List<AdminUserDto>();
            
            foreach (var profile in profiles)
            {
                var appUser = await _userManager.FindByIdAsync(profile.ApplicationUserId.ToString());
                
                users.Add(new AdminUserDto
                {
                    Id = profile.ApplicationUserId,
                    Email = appUser?.Email ?? string.Empty,
                    DisplayName = profile.DisplayName,
                    Role = profile.Role,
                    Status = profile.Status,
                    CreatedAtUtc = profile.CreatedAtUtc
                });
            }

            return users;
        }

        public async Task UpdateUserStatusAsync(Guid userId, UserStatus status, CancellationToken ct)
        {
            var profile = await _userProfileRepo.GetByApplicationUserIdAsync(userId, ct);
            if (profile == null)
            {
                throw new InvalidOperationException("User profile not found");
            }

            if (status == UserStatus.Active)
            {
                profile.Activate();
            }
            else
            {
                profile.Deactivate();
            }
            
            await _userProfileRepo.UpdateAsync(profile, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("User {UserId} status updated to {Status}", userId, status);
        }

        public async Task UpdateUserRoleAsync(Guid userId, UserRole role, CancellationToken ct)
        {
            var profile = await _userProfileRepo.GetByApplicationUserIdAsync(userId, ct);
            if (profile == null)
            {
                throw new InvalidOperationException("User profile not found");
            }

            profile.SetRole(role);
            await _userProfileRepo.UpdateAsync(profile, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("User {UserId} role updated to {Role}", userId, role);
        }

        public async Task<AgencySummaryDto> CreateAgencyAsync(CreateAgencyForAdminDto dto, CancellationToken ct)
        {
            var ownerProfile = await _userProfileRepo.GetByApplicationUserIdAsync(dto.OwnerUserId, ct);
            if (ownerProfile == null)
            {
                throw new InvalidOperationException("Owner user not found");
            }

            var agency = Agency.Create(
                name: dto.Name,
                createdBy: dto.OwnerUserId,
                contactEmail: dto.ContactEmail,
                contactPhone: dto.ContactPhone,
                description: dto.Description
            );

            await _agencyRepo.AddAsync(agency, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Agency {AgencyName} created by admin with owner {OwnerId}", 
                dto.Name, dto.OwnerUserId);

            return new AgencySummaryDto
            {
                Id = agency.Id,
                Name = agency.Name,
                Slug = agency.Slug.Value,
                ContactEmail = agency.ContactEmail,
                ContactPhone = agency.ContactPhone,
                CreatedBy = agency.CreatedBy,
                CreatedAtUtc = agency.CreatedAtUtc
            };
        }

        public async Task<List<AgencySummaryDto>> GetAgenciesAsync(CancellationToken ct)
        {
            var agencies = await _dbContext.Agencies
                .OrderByDescending(a => a.CreatedAtUtc)
                .Select(a => new AgencySummaryDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Slug = a.Slug.Value,
                    ContactEmail = a.ContactEmail,
                    ContactPhone = a.ContactPhone,
                    CreatedBy = a.CreatedBy,
                    CreatedAtUtc = a.CreatedAtUtc
                })
                .ToListAsync(ct);

            return agencies;
        }

        public async Task<StudioSummaryDto> CreateStudioAsync(CreateStudioForAdminDto dto, CancellationToken ct)
        {
            var ownerProfile = await _userProfileRepo.GetByApplicationUserIdAsync(dto.OwnerUserId, ct);
            if (ownerProfile == null)
            {
                throw new InvalidOperationException("Owner user not found");
            }

            var studio = Studio.Create(
                name: dto.Name,
                createdBy: dto.OwnerUserId,
                contactEmail: dto.ContactEmail,
                contactPhone: dto.ContactPhone,
                description: dto.Description
            );

            await _studioRepo.AddAsync(studio, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Studio {StudioName} created by admin with owner {OwnerId}", 
                dto.Name, dto.OwnerUserId);

            return new StudioSummaryDto
            {
                Id = studio.Id,
                Name = studio.Name,
                Slug = studio.Slug.Value,
                ContactEmail = studio.ContactEmail,
                ContactPhone = studio.ContactPhone,
                CreatedBy = studio.CreatedBy,
                CreatedAtUtc = studio.CreatedAtUtc
            };
        }

        public async Task<List<StudioSummaryDto>> GetStudiosAsync(CancellationToken ct)
        {
            var studios = await _dbContext.Studios
                .OrderByDescending(s => s.CreatedAtUtc)
                .Select(s => new StudioSummaryDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Slug = s.Slug.Value,
                    ContactEmail = s.ContactEmail,
                    ContactPhone = s.ContactPhone,
                    CreatedBy = s.CreatedBy,
                    CreatedAtUtc = s.CreatedAtUtc
                })
                .ToListAsync(ct);

            return studios;
        }
    }
}
