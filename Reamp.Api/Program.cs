using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reamp.Infrastructure;
using Reamp.Infrastructure.Identity;

namespace Reamp.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            // 统一读取连接串
            var conn = builder.Configuration.GetConnectionString("SqlServerConnection");

            // DbContext：指定迁移程序集 + 启用重试
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

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseCors("dev");

                // 可选：开发环境自动迁移，省去手动执行
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapGet("/health", () => "OK");

            app.Run();
        }
    }
}