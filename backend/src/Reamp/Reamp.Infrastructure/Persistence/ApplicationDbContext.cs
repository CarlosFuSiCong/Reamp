using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Common.Abstractions;
using Reamp.Domain.Listings.Entities;
using Reamp.Domain.Media.Entities;
using Reamp.Domain.Shoots.Entities;
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
        public DbSet<Agent> Agents { get; set; } = null!;
        public DbSet<Studio> Studios { get; set; } = null!;
        public DbSet<Staff> Staffs { get; set; } = null!;
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Invitation> Invitations { get; set; } = null!;
        public DbSet<OrganizationApplication> OrganizationApplications { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        // Media
        public DbSet<MediaAsset> MediaAssets { get; set; } = null!;
        public DbSet<MediaVariant> MediaVariants { get; set; } = null!;

        // Shoots (Orders)
        public DbSet<ShootOrder> ShootOrders { get; set; } = null!;
        public DbSet<ShootTask> ShootTasks { get; set; } = null!;
        
        // Listings
        public DbSet<Listing> Listings { get; set; } = null!;
        public DbSet<ListingMediaRef> ListingMediaRefs { get; set; } = null!;
        public DbSet<ListingAgentSnapshot> ListingAgentSnapshots { get; set; } = null!;

        // Delivery
        public DbSet<Reamp.Domain.Delivery.Entities.DeliveryPackage> DeliveryPackages { get; set; } = null!;
        public DbSet<Reamp.Domain.Delivery.Entities.DeliveryItem> DeliveryItems { get; set; } = null!;
        public DbSet<Reamp.Domain.Delivery.Entities.DeliveryAccess> DeliveryAccess { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply all IEntityTypeConfiguration<T> (AgencyConfiguration, StudioConfiguration, ShootOrderConfiguration, etc.)
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Apply soft delete query filters
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
                if (entry.Entity is ISoftDeletable sd && entry.State == EntityState.Deleted)
                {
                    sd.SoftDelete();
                    entry.State = EntityState.Modified;
                }

                if (entry.Entity is IAuditableEntity a &&
                    (entry.State == EntityState.Added || entry.State == EntityState.Modified))
                {
                    if (entry.State == EntityState.Added)
                        a.MarkCreated();

                    a.MarkUpdated();
                }
            }
        }
    }
}