using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Reamp.Application.Authentication;
using Serilog;
using System.Text;

namespace Reamp.Api.Configuration
{
    public static class JwtConfiguration
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
            if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
                throw new InvalidOperationException("JWT settings are not configured properly.");

            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        ClockSkew = TimeSpan.Zero,
                        NameClaimType = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,
                        RoleClaimType = "role"
                    };

                    options.Events = CreateJwtBearerEvents();
                });

            return services;
        }

        private static JwtBearerEvents CreateJwtBearerEvents()
        {
            return new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Cookies["accessToken"];
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                        Log.Debug("JWT token retrieved from cookie");
                    }
                    else
                    {
                        var authHeader = context.Request.Headers["Authorization"].ToString();
                        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = authHeader.Substring("Bearer ".Length).Trim();
                            Log.Debug("JWT token retrieved from Authorization header");
                        }
                        else
                        {
                            Log.Debug("No JWT token found in cookie or Authorization header");
                        }
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    Log.Warning("JWT authentication failed: {Error} for path: {Path}", 
                        context.Exception?.Message ?? "Unknown error", context.Request.Path);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var allClaims = context.Principal?.Claims.Select(c => $"{c.Type}={c.Value}").ToList();
                    var userId = context.Principal?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
                    Log.Debug("JWT token validated. UserId: {UserId}, Claims: {Claims}, Path: {Path}", 
                        userId ?? "unknown", 
                        allClaims != null ? string.Join(", ", allClaims) : "none",
                        context.Request.Path);
                    return Task.CompletedTask;
                }
            };
        }
    }
}
