using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reamp.Api.Configuration;
using Reamp.Application.Accounts.Agencies.Services;
using Reamp.Application.Accounts.Clients.Services;
using Reamp.Application.Accounts.Staff.Services;
using Reamp.Application.Accounts.Studios.Services;
using Reamp.Application.Accounts.Studios.Validators;
using Reamp.Application.Authentication;
using Reamp.Application.Authentication.Services;
using Reamp.Application.Listings.Services;
using Reamp.Application.Orders.Services;
using Reamp.Application.Read.Agencies;
using Reamp.Application.Read.Clients;
using Reamp.Application.Read.Listings;
using Reamp.Application.Read.Staff;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Listings.Repositories;
using Reamp.Domain.Orders.Repositories;
using Reamp.Infrastructure;
using Reamp.Infrastructure.Identity;
using Reamp.Infrastructure.Read.EF.Agencies;
using Reamp.Infrastructure.Read.EF.Clients;
using Reamp.Infrastructure.Read.EF.Listings;
using Reamp.Infrastructure.Read.EF.Staff;
using Reamp.Infrastructure.Repositories.Accounts;
using Reamp.Infrastructure.Repositories.Common;
using Reamp.Infrastructure.Repositories.Listings;
using Reamp.Infrastructure.Repositories.Orders;
using Reamp.Shared;
using Reamp.Shared.Middlewares;
using Serilog;

namespace Reamp.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Disable automatic claim type mapping to preserve original JWT claim names
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var builder = WebApplication.CreateBuilder(args);

            // Logging
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();
            builder.Host.UseSerilog();

            // API & Swagger
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Reamp API",
                    Version = "v1",
                    Description = "Real Estate Photography Marketplace API"
                });

                // Add JWT Authentication support
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter your token in the text input below.",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // CORS
            var allowedOrigins = builder.Configuration.GetValue<string>("AllowedOrigins");
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    if (!string.IsNullOrEmpty(allowedOrigins))
                    {
                        // Production: use configured origins
                        var origins = allowedOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.Trim())
                            .ToArray();
                        policy.WithOrigins(origins)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    }
                    else if (builder.Environment.IsDevelopment())
                    {
                        // Development: default to localhost:3000
                        policy.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                              .AllowCredentials();
                    }
                    else
                    {
                        // Fallback: allow all origins (not recommended for production)
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    }
                });
            });

            // Database
            var conn = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(conn, sql =>
                {
                    sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    sql.EnableRetryOnFailure();
                })
            );

            // Background Jobs
            builder.Services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(conn));
            builder.Services.AddHangfireServer();

            // Real-time
            builder.Services.AddSignalR();

            // Identity
            builder.Services
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configuration
            builder.Services.Configure<Reamp.Infrastructure.Configuration.CloudinarySettings>(
                builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.Configure<Reamp.Infrastructure.Configuration.MediaUploadSettings>(
                builder.Configuration.GetSection("MediaUploadSettings"));

            // Authentication & Authorization
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthPolicies.RequireAdminRole, policy =>
                    policy.RequireRole("Admin"));
                options.AddPolicy(AuthPolicies.RequireStaffRole, policy =>
                    policy.RequireRole("Staff"));
                options.AddPolicy(AuthPolicies.RequireClientRole, policy =>
                    policy.RequireRole("Client"));
                options.AddPolicy(AuthPolicies.RequireUserRole, policy =>
                    policy.RequireRole("User"));
                options.AddPolicy(AuthPolicies.RequireAgentRole, policy =>
                    policy.RequireRole("Agent"));
                options.AddPolicy(AuthPolicies.RequireStaffOrAdmin, policy =>
                    policy.RequireRole("Staff", "Admin"));
                options.AddPolicy(AuthPolicies.RequireClientOrAdmin, policy =>
                    policy.RequireRole("Client", "Admin"));
                options.AddPolicy(AuthPolicies.RequireAgentOrAdmin, policy =>
                    policy.RequireRole("Agent", "Admin"));
            });

            // Validation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateStudioDtoValidator>();

            // Object Mapping
            var mapsterConfig = TypeAdapterConfig.GlobalSettings;
            Reamp.Application.Common.Mappings.MappingConfig.RegisterMappings();
            builder.Services.AddSingleton(mapsterConfig);

            // Repositories
            builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            builder.Services.AddScoped<IAgencyRepository, AgencyRepository>();
            builder.Services.AddScoped<IAgencyBranchRepository, AgencyBranchRepository>();
            builder.Services.AddScoped<IAgentRepository, Reamp.Infrastructure.Repositories.Accounts.AgentRepository>();
            builder.Services.AddScoped<IStudioRepository, StudioRepository>();
            builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IStaffRepository, StaffRepository>();
            builder.Services.AddScoped<Reamp.Domain.Accounts.Repositories.IInvitationRepository,
                Reamp.Infrastructure.Repositories.Accounts.InvitationRepository>();
            builder.Services.AddScoped<IOrganizationApplicationRepository,
                OrganizationApplicationRepository>();
            builder.Services.AddScoped<IListingRepository, ListingRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<Reamp.Domain.Media.Repositories.IMediaAssetRepository,
                Reamp.Infrastructure.Repositories.Media.MediaAssetRepository>();
            builder.Services.AddScoped<IShootOrderRepository, ShootOrderRepository>();
            builder.Services.AddScoped<Reamp.Domain.Delivery.Repositories.IDeliveryPackageRepository,
                Reamp.Infrastructure.Repositories.Delivery.DeliveryPackageRepository>();

            // Application Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<Reamp.Application.Admin.Services.IAdminService,
                Reamp.Application.Admin.Services.AdminService>();
            builder.Services.AddScoped<Reamp.Application.Applications.Services.IApplicationService,
                Reamp.Application.Applications.Services.ApplicationService>();
            builder.Services.AddScoped<IAgencyAppService, AgencyAppService>();
            builder.Services.AddScoped<IClientAppService, ClientAppService>();
            builder.Services.AddScoped<IStaffAppService, StaffAppService>();
            builder.Services.AddScoped<IStudioAppService, StudioAppService>();
            builder.Services.AddScoped<IListingAppService, ListingAppService>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<Reamp.Application.Media.Services.IMediaAssetAppService,
                Reamp.Application.Media.Services.MediaAssetAppService>();
            builder.Services.AddScoped<Reamp.Application.Media.Services.IChunkedUploadService,
                Reamp.Application.Media.Services.ChunkedUploadService>();
            builder.Services.AddScoped<Reamp.Application.Media.Services.IMediaProcessingJob,
                Reamp.Application.Media.Services.MediaProcessingJob>();
            builder.Services.AddScoped<IShootOrderAppService, ShootOrderAppService>();
            builder.Services.AddScoped<Reamp.Application.Delivery.Services.IDeliveryPackageAppService,
                Reamp.Application.Delivery.Services.DeliveryPackageAppService>();
            builder.Services.AddScoped<Reamp.Application.UserProfiles.Services.IUserProfileAppService,
                Reamp.Application.UserProfiles.Services.UserProfileAppService>();
            builder.Services.AddScoped<Reamp.Application.Invitations.Services.IInvitationAppService,
                Reamp.Application.Invitations.Services.InvitationAppService>();
            builder.Services.AddScoped<Reamp.Application.Members.Services.IMemberAppService,
                Reamp.Application.Members.Services.MemberAppService>();
            builder.Services.AddScoped<Reamp.Domain.Common.Services.IBackgroundJobService,
                Reamp.Infrastructure.Services.Jobs.HangfireBackgroundJobService>();

            // Query Services
            builder.Services.AddScoped<Reamp.Application.Common.Services.IAccountQueryService,
                Reamp.Application.Common.Services.AccountQueryService>();
            builder.Services.AddScoped<Reamp.Application.Common.Services.IPermissionService,
                Reamp.Application.Common.Services.PermissionService>();

            // Media Services
            builder.Services.AddScoped<Reamp.Infrastructure.Services.Media.ICloudinaryService, 
                Reamp.Infrastructure.Services.Media.CloudinaryService>();
            builder.Services.AddSingleton<Reamp.Infrastructure.Services.Media.IUploadSessionStore,
                Reamp.Infrastructure.Services.Media.InMemoryUploadSessionStore>();
            builder.Services.AddHostedService<Reamp.Infrastructure.Services.Media.UploadSessionCleanupService>();

            // Read Services
            builder.Services.AddScoped<IAgencyReadService, EfAgencyReadService>();
            builder.Services.AddScoped<IClientReadService, EfClientReadService>();
            builder.Services.AddScoped<IStaffReadService, EfStaffReadService>();
            builder.Services.AddScoped<IListingReadService, EfListingReadService>();

            var app = builder.Build();

            app.UseSerilogRequestLogging();

            // Swagger - controlled by environment variable
            var enableSwagger = builder.Configuration.GetValue<bool?>("EnableSwagger");
            if (enableSwagger ?? app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reamp API v1");
                    c.DocumentTitle = "Reamp API Documentation";
                });
            }

            // Run migrations on startup (controlled by environment variable)
            var autoMigrate = builder.Configuration.GetValue<bool?>("Database:AutoMigrate") ?? false;
            if (autoMigrate)
            {
                using var scope = app.Services.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                try
                {
                    logger.LogInformation("Auto-migration is enabled. Acquiring migration lock...");
                    
                    // Use application lock to ensure only one instance runs migrations
                    using var connection = db.Database.GetDbConnection();
                    await connection.OpenAsync();
                    using var command = connection.CreateCommand();
                    
                    // Acquire exclusive lock (timeout: 10 seconds)
                    command.CommandText = "DECLARE @result INT; EXEC @result = sp_getapplock @Resource='ReampDbMigration', @LockMode='Exclusive', @LockOwner='Session', @LockTimeout=10000; SELECT @result";
                    var lockResult = await command.ExecuteScalarAsync();
                    
                    if (Convert.ToInt32(lockResult) < 0)
                    {
                        logger.LogWarning("Could not acquire migration lock. Another instance may be running migrations.");
                        return;
                    }
                    
                    logger.LogInformation("Migration lock acquired. Checking database migrations...");
                    var pendingMigrations = db.Database.GetPendingMigrations().ToList();
                    
                    if (pendingMigrations.Any())
                    {
                        logger.LogWarning("Found {Count} pending migrations: {Migrations}", 
                            pendingMigrations.Count, string.Join(", ", pendingMigrations));
                        logger.LogInformation("Applying migrations...");
                        db.Database.Migrate();
                        logger.LogInformation("Migrations applied successfully");
                    }
                    else
                    {
                        logger.LogInformation("Database is up to date - no pending migrations");
                    }
                    
                    // Release lock
                    command.CommandText = "EXEC sp_releaseapplock @Resource='ReampDbMigration', @LockOwner='Session'";
                    await command.ExecuteNonQueryAsync();
                    logger.LogInformation("Migration lock released");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error during database migration");
                    throw;
                }
            }
            else
            {
                using var scope = app.Services.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Auto-migration is disabled (Database:AutoMigrate = false)");
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            
            // CORS must be after UseRouting (implicit) and before UseAuthentication
            app.UseRouting();
            app.UseCors("AllowFrontend");
            
            app.UseAuthentication();
            app.UseAuthorization();

            // Hangfire Dashboard - controlled by environment variable
            var enableHangfire = builder.Configuration.GetValue<bool?>("EnableHangfire");
            if (enableHangfire ?? app.Environment.IsDevelopment())
            {
                app.UseHangfireDashboard("/hangfire");
            }

            app.MapControllers();
            app.MapHub<Reamp.Api.Hubs.UploadProgressHub>("/hubs/upload-progress");
            app.MapGet("/health", () => "OK");

            app.Run();
        }
    }
}
