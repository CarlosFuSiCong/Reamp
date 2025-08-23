using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Common.Interfaces;
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
            : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            builder.ApplySoftDeleteQueryFilters();
        }

        public override int SaveChanges()
        {
            ApplyAuditInfoAndSoftDelete();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfoAndSoftDelete();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInfoAndSoftDelete()
        {
            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                if (entry.Entity is ISoftDeletable sd && entry.State == EntityState.Deleted)
                {
                    sd.SoftDelete();
                    entry.State = EntityState.Modified;
                }

                if (entry.Entity is IAuditableEntity a &&
                   (entry.State == EntityState.Added || entry.State == EntityState.Modified))
                {
                    if (entry.State == EntityState.Added) a.MarkCreated();
                    a.MarkUpdated();
                }
            }
        }
    }
}