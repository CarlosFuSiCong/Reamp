using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reamp.Domain.Delivery.Entities;
using Reamp.Domain.Media.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Configurations.Delivery
{
    public class DeliveryItemConfiguration : IEntityTypeConfiguration<DeliveryItem>
    {
        public void Configure(EntityTypeBuilder<DeliveryItem> b)
        {
            b.ToTable("DeliveryItems");

            b.HasKey(x => x.Id);

            b.Property(x => x.DeliveryPackageId).IsRequired();
            b.Property(x => x.MediaAssetId).IsRequired();
            b.Property(x => x.VariantName).IsRequired().HasMaxLength(64);
            b.Property(x => x.SortOrder).HasDefaultValue(0);

            b.HasIndex(x => x.DeliveryPackageId);
            b.HasIndex(x => x.MediaAssetId);

            b.HasOne<DeliveryPackage>()
             .WithMany(p => (ICollection<DeliveryItem>)p.Items)
             .HasForeignKey(x => x.DeliveryPackageId)
             .OnDelete(DeleteBehavior.Cascade);

            // Do NOT cascade delete media
            // Explicitly map the MediaAsset navigation property
            b.HasOne(x => x.MediaAsset)
             .WithMany()
             .HasForeignKey(x => x.MediaAssetId)
             .OnDelete(DeleteBehavior.NoAction);

            // package-level uniqueness: avoid duplicate asset+variant
            b.HasIndex(x => new { x.DeliveryPackageId, x.MediaAssetId, x.VariantName }).IsUnique();
        }
    }
}