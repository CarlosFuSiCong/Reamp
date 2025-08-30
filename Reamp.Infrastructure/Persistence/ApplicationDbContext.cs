using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Common.Abstractions;
using Reamp.Infrastructure.Extensions;
using Reamp.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Accounts
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<Agency> Agencies { get; set; } = null!;
        public DbSet<AgencyBranch> AgencyBranches { get; set; } = null!;
        public DbSet<Studio> Studios { get; set; } = null!;
        public DbSet<Staff> Staffs { get; set; } = null!;
        public DbSet<Client> Clients { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 扫描并应用所有 IEntityTypeConfiguration<T>（如 AgencyConfiguration 等）
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // 统一软删除查询过滤（你已有扩展方法）
            builder.ApplySoftDeleteQueryFilters();
        }

        public override int SaveChanges()
        {
            ApplyAuditInfoAndSoftDelete();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfoAndSoftDelete();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInfoAndSoftDelete()
        {
            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                // 统一软删除：把 Delete 变为 SoftDelete + Modified
                if (entry.Entity is ISoftDeletable sd && entry.State == EntityState.Deleted)
                {
                    sd.SoftDelete();
                    entry.State = EntityState.Modified;
                }

                // 统一审计：创建时 MarkCreated，创建和修改时 MarkUpdated
                if (entry.Entity is IAuditableEntity a &&
                    (entry.State == EntityState.Added || entry.State == EntityState.Modified))
                {
                    if (entry.State == EntityState.Added)
                        a.MarkCreated();

                    a.MarkUpdated(); // 若你希望“新增时不标记 Updated”，可将此行改为：if (entry.State == EntityState.Modified) a.MarkUpdated();
                }
            }
        }
    }
}