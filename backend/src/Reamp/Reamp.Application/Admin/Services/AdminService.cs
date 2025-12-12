using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly IAgentRepository _agentRepo;
        private readonly IStaffRepository _staffRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            ApplicationDbContext dbContext,
            IUserProfileRepository userProfileRepo,
            IAgencyRepository agencyRepo,
            IStudioRepository studioRepo,
            IAgentRepository agentRepo,
            IStaffRepository staffRepo,
            UserManager<ApplicationUser> userManager,
            IUnitOfWork uow,
            ILogger<AdminService> logger)
        {
            _dbContext = dbContext;
            _userProfileRepo = userProfileRepo;
            _agencyRepo = agencyRepo;
            _studioRepo = studioRepo;
            _agentRepo = agentRepo;
            _staffRepo = staffRepo;
            _userManager = userManager;
            _uow = uow;
            _logger = logger;
        }

        public async Task<AdminStatsResponse> GetStatsAsync(CancellationToken ct)
        {
            // Batch all count queries
            var countsTask = Task.WhenAll(
                _dbContext.UserProfiles.CountAsync(ct),
                _dbContext.Listings.CountAsync(l => l.Status == ListingStatus.Active, ct),
                _dbContext.ShootOrders.CountAsync(ct),
                _dbContext.Studios.CountAsync(ct),
                _dbContext.ShootOrders.CountAsync(o => o.Status == ShootOrderStatus.Placed, ct),
                _dbContext.Listings.CountAsync(l => l.Status == ListingStatus.Pending, ct)
            );

            // Get chart data for last 7 days in single query
            var sixDaysAgo = DateTime.UtcNow.AddDays(-6).Date;
            var ordersGrouped = await _dbContext.ShootOrders
                .Where(o => o.CreatedAtUtc >= sixDaysAgo)
                .GroupBy(o => o.CreatedAtUtc.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            var listingsGrouped = await _dbContext.Listings
                .Where(l => l.CreatedAtUtc >= sixDaysAgo)
                .GroupBy(l => l.CreatedAtUtc.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            // Get recent activities in single query
            var recentActivitiesTask = Task.WhenAll(
                _dbContext.ShootOrders
                    .OrderByDescending(o => o.CreatedAtUtc)
                    .Take(5)
                    .Select(o => new ActivityDto
                    {
                        Id = o.Id.ToString(),
                        Type = "order",
                        Title = "New Order",
                        Description = "New order placed",
                        Timestamp = o.CreatedAtUtc
                    })
                    .ToListAsync(ct),
                _dbContext.Listings
                    .OrderByDescending(l => l.CreatedAtUtc)
                    .Take(5)
                    .Select(l => new ActivityDto
                    {
                        Id = l.Id.ToString(),
                        Type = "listing",
                        Title = "New Listing",
                        Description = "New listing created",
                        Timestamp = l.CreatedAtUtc
                    })
                    .ToListAsync(ct)
            );

            // Wait for all queries to complete
            var counts = await countsTask;
            var recentActivities = await recentActivitiesTask;

            var totalUsers = counts[0];
            var activeListings = counts[1];
            var totalOrders = counts[2];
            var totalStudios = counts[3];
            var pendingOrders = counts[4];
            var pendingListings = counts[5];

            // Build chart data from grouped results
            var ordersDict = ordersGrouped.ToDictionary(x => x.Date, x => x.Count);
            var listingsDict = listingsGrouped.ToDictionary(x => x.Date, x => x.Count);
            var chartData = new List<ChartDataPoint>();

            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.UtcNow.AddDays(-i).Date;
                chartData.Add(new ChartDataPoint
                {
                    Date = date.ToString("MM/dd"),
                    Orders = ordersDict.GetValueOrDefault(date, 0),
                    Listings = listingsDict.GetValueOrDefault(date, 0)
                });
            }

            // Generate system alerts
            var alerts = new List<SystemAlert>();
            if (pendingOrders > 0)
            {
                alerts.Add(new SystemAlert
                {
                    Title = "Pending Orders",
                    Message = $"{pendingOrders} order(s) waiting for acceptance"
                });
            }
            if (pendingListings > 0)
            {
                alerts.Add(new SystemAlert
                {
                    Title = "Pending Listings",
                    Message = $"{pendingListings} listing(s) awaiting approval"
                });
            }

            var activities = recentActivities[0].Concat(recentActivities[1])
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
            var users = await (
                from profile in _dbContext.UserProfiles
                join appUser in _dbContext.Users on profile.ApplicationUserId equals appUser.Id
                orderby profile.CreatedAtUtc descending
                select new AdminUserDto
                {
                    Id = profile.ApplicationUserId,
                    Email = appUser.Email ?? string.Empty,
                    DisplayName = profile.DisplayName,
                    Role = profile.Role,
                    Status = profile.Status,
                    CreatedAtUtc = profile.CreatedAtUtc
                }
            ).ToListAsync(ct);

            return users;
        }

        public async Task UpdateUserStatusAsync(Guid userId, UserStatus status, CancellationToken ct)
        {
            var profile = await _userProfileRepo.GetByApplicationUserIdAsync(userId, false, false, ct);
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
            
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("User {UserId} status updated to {Status}", userId, status);
        }

        public async Task UpdateUserRoleAsync(Guid userId, UserRole role, CancellationToken ct)
        {
            var profile = await _userProfileRepo.GetByApplicationUserIdAsync(userId, false, false, ct);
            if (profile == null)
            {
                throw new InvalidOperationException("User profile not found");
            }

            profile.SetRole(role);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("User {UserId} role updated to {Role}", userId, role);
        }

        public async Task<AgencySummaryDto> CreateAgencyAsync(CreateAgencyForAdminDto dto, CancellationToken ct)
        {
            var ownerProfile = await _userProfileRepo.GetByApplicationUserIdAsync(dto.OwnerUserId, false, true, ct);
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

            // Automatically create Agent record with Owner role
            var ownerAgent = new Agent(ownerProfile.Id, agency.Id, AgencyRole.Owner);
            await _agentRepo.AddAsync(ownerAgent, ct);
            
            // Save both agency and owner agent in a single transaction
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
            var ownerProfile = await _userProfileRepo.GetByApplicationUserIdAsync(dto.OwnerUserId, false, true, ct);
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

            // Automatically create Staff record with Owner role
            var ownerStaff = new Domain.Accounts.Entities.Staff(ownerProfile.Id, studio.Id, StudioRole.Owner);
            await _staffRepo.AddAsync(ownerStaff, ct);
            
            // Save both studio and owner staff in a single transaction
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
