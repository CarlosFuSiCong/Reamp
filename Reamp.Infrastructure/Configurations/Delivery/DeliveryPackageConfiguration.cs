using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reamp.Domain.Delivery.Entities;
using Reamp.Domain.Delivery.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Configurations.Delivery
{
    public class DeliveryPackageConfiguration : IEntityTypeConfiguration<DeliveryPackage>
    {
        public void Configure(EntityTypeBuilder<DeliveryPackage> b)
        {
            b.ToTable("DeliveryPackages", t =>
            {
                t.HasCheckConstraint("CK_DeliveryPackages_Title_NotEmpty",
                    "LEN(LTRIM(RTRIM([Title]))) > 0");
            });

            b.HasKey(x => x.Id);

            b.Property(x => x.OrderId).IsRequired();
            b.Property(x => x.ListingId).IsRequired();

            b.Property(x => x.Title).IsRequired().HasMaxLength(160);
            b.Property(x => x.Status).HasConversion<int>().IsRequired();
            b.Property(x => x.WatermarkEnabled).IsRequired();
            b.Property(x => x.ExpiresAtUtc);

            b.HasIndex(x => x.OrderId);
            b.HasIndex(x => x.ListingId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.ExpiresAtUtc);

            b.HasQueryFilter(x => x.DeletedAtUtc == null);

            // strong FKs
            b.HasOne<Reamp.Domain.Shoots.Entities.ShootOrder>()
             .WithMany()
             .HasForeignKey(x => x.OrderId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<Reamp.Domain.Listings.Entities.Listing>()
             .WithMany()
             .HasForeignKey(x => x.ListingId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Navigation(x => x.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
            b.Navigation(x => x.Accesses).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}