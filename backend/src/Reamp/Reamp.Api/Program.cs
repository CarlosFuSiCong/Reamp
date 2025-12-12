using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
using Reamp.Domain.Shoots.Repositories;
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
using System.Text;

namespace Reamp.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Logging
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();
            builder.Host.UseSerilog();

            // API & Swagger
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // CORS
            builder.Services.AddCors(o =>
                o.AddPolicy("dev", p => p
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                )
            );

            // Database
            var conn = builder.Configuration.GetConnectionString("SqlServerConnection");
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
            var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
            if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
                throw new InvalidOperationException("JWT settings are not configured properly.");

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
            builder.Services.Configure<Reamp.Infrastructure.Configuration.CloudinarySettings>(
                builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.Configure<Reamp.Infrastructure.Configuration.MediaUploadSettings>(
                builder.Configuration.GetSection("MediaUploadSettings"));

            // Authentication
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // Try to read token from cookie first, fallback to Authorization header
                            var accessToken = context.Request.Cookies["accessToken"];
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            Log.Warning("JWT authentication failed: {Error}", context.Exception.Message);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Log.Debug("JWT token validated for user: {UserId}", context.Principal?.Identity?.Name);
                            return Task.CompletedTask;
                        }
                    };
                });

            // Authorization
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
                options.AddPolicy(AuthPolicies.RequireStaffOrAdmin, policy =>
                    policy.RequireRole("Staff", "Admin"));
                options.AddPolicy(AuthPolicies.RequireClientOrAdmin, policy =>
                    policy.RequireRole("Client", "Admin"));
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
            builder.Services.AddScoped<IStudioRepository, StudioRepository>();
            builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IStaffRepository, StaffRepository>();
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
            builder.Services.AddScoped<Reamp.Domain.Common.Services.IBackgroundJobService,
                Reamp.Infrastructure.Services.Jobs.HangfireBackgroundJobService>();

            // Query Services
            builder.Services.AddScoped<Reamp.Application.Common.Services.IAccountQueryService,
                Reamp.Application.Common.Services.AccountQueryService>();

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

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseCors("dev");

                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
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
