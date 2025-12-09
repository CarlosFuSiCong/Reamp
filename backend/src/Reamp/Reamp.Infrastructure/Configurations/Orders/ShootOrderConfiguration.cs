using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Listings.Entities;
using Reamp.Domain.Shoots.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Configurations.Shoots
{
    public class ShootOrderConfiguration : IEntityTypeConfiguration<ShootOrder>
    {
        public void Configure(EntityTypeBuilder<ShootOrder> b)
        {
            b.ToTable("ShootOrders", t =>
            {
                t.HasCheckConstraint("CK_ShootOrders_Total_NotNegative", "[TotalAmount] >= 0");
            });

            b.HasKey(x => x.Id);

            b.Property(x => x.AgencyId).IsRequired();
            b.Property(x => x.StudioId).IsRequired();
            b.Property(x => x.ListingId).IsRequired();

            b.Property(x => x.Currency).IsRequired().HasMaxLength(3);
            b.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
            b.Property(x => x.Status).HasConversion<int>().IsRequired();
            b.Property(x => x.CreatedBy).IsRequired();
            b.Property(x => x.CancellationReason).HasMaxLength(500);

            b.HasIndex(x => x.AgencyId);
            b.HasIndex(x => x.StudioId);
            b.HasIndex(x => x.ListingId);
            b.HasIndex(x => x.Status);

            // Optional: enable soft-delete filter if AuditableEntity has DeletedAtUtc
            b.HasQueryFilter(x => x.DeletedAtUtc == null);

            // Nav collection uses backing field
            b.Navigation(x => x.Tasks).UsePropertyAccessMode(PropertyAccessMode.Field);

            // in ShootOrderConfiguration
            b.HasOne<Agency>().WithMany()
             .HasForeignKey(x => x.AgencyId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<Studio>().WithMany()
             .HasForeignKey(x => x.StudioId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<Listing>().WithMany()
             .HasForeignKey(x => x.ListingId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}