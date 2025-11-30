using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
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
using Reamp.Application.Read.Agencies;
using Reamp.Application.Read.Clients;
using Reamp.Application.Read.Listings;
using Reamp.Application.Read.Staff;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Listings.Repositories;
using Reamp.Infrastructure;
using Reamp.Infrastructure.Identity;
using Reamp.Infrastructure.Read.EF.Agencies;
using Reamp.Infrastructure.Read.EF.Clients;
using Reamp.Infrastructure.Read.EF.Listings;
using Reamp.Infrastructure.Read.EF.Staff;
using Reamp.Infrastructure.Repositories.Accounts;
using Reamp.Infrastructure.Repositories.Common;
using Reamp.Infrastructure.Repositories.Listings;
using Reamp.Shared;
using Serilog;
using System.Text;

namespace Reamp.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---- Serilog: �� appsettings.json ��ȡ���ã����ӹ���־ ----
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();
            builder.Host.UseSerilog();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(o =>
                o.AddPolicy("dev", p => p
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                )
            );

            // DbContext
            var conn = builder.Configuration.GetConnectionString("SqlServerConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(conn, sql =>
                {
                    sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    sql.EnableRetryOnFailure();
                })
            );

            // Hangfire (Background Jobs)
            builder.Services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(conn));

            builder.Services.AddHangfireServer();

            // SignalR
            builder.Services.AddSignalR();

            // Identity
            builder.Services
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // JWT Settings
            var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
            if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
                throw new InvalidOperationException("JWT settings are not configured properly.");

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

            // Cloudinary Settings
            builder.Services.Configure<Reamp.Infrastructure.Configuration.CloudinarySettings>(
                builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.Configure<Reamp.Infrastructure.Configuration.MediaUploadSettings>(
                builder.Configuration.GetSection("MediaUploadSettings"));

            // JWT Authentication
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
                        ClockSkew = TimeSpan.Zero // No tolerance for expiration
                    };

                    // Log JWT events for debugging (optional)
                    options.Events = new JwtBearerEvents
                    {
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

            // Authorization Policies
            builder.Services.AddAuthorization(options =>
            {
                // Admin only
                options.AddPolicy(AuthPolicies.RequireAdminRole, policy =>
                    policy.RequireRole("Admin"));

                // Staff only
                options.AddPolicy(AuthPolicies.RequireStaffRole, policy =>
                    policy.RequireRole("Staff"));

                // Client only
                options.AddPolicy(AuthPolicies.RequireClientRole, policy =>
                    policy.RequireRole("Client"));

                // User only
                options.AddPolicy(AuthPolicies.RequireUserRole, policy =>
                    policy.RequireRole("User"));

                // Staff or Admin
                options.AddPolicy(AuthPolicies.RequireStaffOrAdmin, policy =>
                    policy.RequireRole("Staff", "Admin"));

                // Client or Admin
                options.AddPolicy(AuthPolicies.RequireClientOrAdmin, policy =>
                    policy.RequireRole("Client", "Admin"));
            });

            // FluentValidation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateStudioDtoValidator>();

            // UoW + �ִ�
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

            // Application Services
            builder.Services.AddScoped<IAuthService, AuthService>();
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

            // Media Services
            builder.Services.AddScoped<Reamp.Infrastructure.Services.Media.ICloudinaryService, 
                Reamp.Infrastructure.Services.Media.CloudinaryService>();
            builder.Services.AddSingleton<Reamp.Infrastructure.Services.Media.IUploadSessionStore,
                Reamp.Infrastructure.Services.Media.InMemoryUploadSessionStore>();

            // Read Services  
            builder.Services.AddScoped<IAgencyReadService, EfAgencyReadService>();
            builder.Services.AddScoped<IClientReadService, EfClientReadService>();
            builder.Services.AddScoped<IStaffReadService, EfStaffReadService>();
            builder.Services.AddScoped<IListingReadService, EfListingReadService>();

            var app = builder.Build();

            // Serilog ������־��������ʱ/״̬��ȣ�
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseCors("dev");

                // Auto migration in development
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

            app.UseHttpsRedirection();
            
            // Authentication must come before Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Global exception handling middleware
            app.Use(async (ctx, next) =>
            {
                try 
                { 
                    await next(); 
                }
                catch (UnauthorizedAccessException ex)
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await ctx.Response.WriteAsJsonAsync(ApiResponse.Fail(ex.Message));
                }
                catch (ArgumentException ex)
                {
                    ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await ctx.Response.WriteAsJsonAsync(ApiResponse.Fail(ex.Message, "Invalid request"));
                }
                catch (InvalidOperationException ex)
                {
                    ctx.Response.StatusCode = StatusCodes.Status409Conflict;
                    await ctx.Response.WriteAsJsonAsync(ApiResponse.Fail(ex.Message, "Operation conflict"));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unhandled exception occurred");
                    ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await ctx.Response.WriteAsJsonAsync(ApiResponse.Fail("An unexpected error occurred", "Internal server error"));
                }
            });

            // Hangfire Dashboard (no auth for development)
            app.UseHangfireDashboard("/hangfire");

            app.MapControllers();
            app.MapHub<Reamp.Api.Hubs.UploadProgressHub>("/hubs/upload-progress");
            app.MapGet("/health", () => "OK");

            app.Run();
        }
    }
}