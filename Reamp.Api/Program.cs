using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reamp.Application.Accounts.Studios.Services;
using Reamp.Application.Accounts.Studios.Validators;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Infrastructure;
using Reamp.Infrastructure.Identity;
using Reamp.Infrastructure.Repositories.Accounts;
using Reamp.Infrastructure.Repositories.Common;
using Serilog;

namespace Reamp.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---- Serilog: 从 appsettings.json 读取配置，并接管日志 ----
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

            // Identity
            builder.Services
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // FluentValidation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateStudioDtoValidator>();

            // UoW + 仓储
            builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            builder.Services.AddScoped<IAgencyRepository, AgencyRepository>();
            builder.Services.AddScoped<IAgencyBranchRepository, AgencyBranchRepository>();
            builder.Services.AddScoped<IStudioRepository, StudioRepository>();
            builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IStaffRepository, StaffRepository>();

            // Application Services
            builder.Services.AddScoped<IStudioAppService, StudioAppService>();

            var app = builder.Build();

            // Serilog 请求日志（包含耗时/状态码等）
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseCors("dev");

                // 可选：开发环境自动迁移
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // 简单的全局异常到 JSON（可换成你自己的中间件）
            app.Use(async (ctx, next) =>
            {
                try { await next(); }
                catch (ArgumentException ex)
                {
                    ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await ctx.Response.WriteAsJsonAsync(new { error = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    ctx.Response.StatusCode = StatusCodes.Status409Conflict;
                    await ctx.Response.WriteAsJsonAsync(new { error = ex.Message });
                }
            });

            app.MapControllers();
            app.MapGet("/health", () => "OK");

            app.Run();
        }
    }
}