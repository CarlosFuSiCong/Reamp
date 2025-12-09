using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reamp.Domain.Delivery.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Configurations.Delivery
{
    public class DeliveryAccessConfiguration : IEntityTypeConfiguration<DeliveryAccess>
    {
        public void Configure(EntityTypeBuilder<DeliveryAccess> b)
        {
            b.ToTable("DeliveryAccess");

            b.HasKey(x => x.Id);

            b.Property(x => x.DeliveryPackageId).IsRequired();
            b.Property(x => x.Type).HasConversion<int>().IsRequired();
            b.Property(x => x.RecipientEmail).HasMaxLength(256);
            b.Property(x => x.RecipientName).HasMaxLength(120);
            b.Property(x => x.PasswordHash).HasMaxLength(256);

            b.Property(x => x.MaxDownloads);
            b.Property(x => x.Downloads).HasDefaultValue(0);

            b.HasIndex(x => x.DeliveryPackageId);
            b.HasIndex(x => new { x.DeliveryPackageId, x.Type });

            b.HasOne<DeliveryPackage>()
             .WithMany(p => (ICollection<DeliveryAccess>)p.Accesses)
             .HasForeignKey(x => x.DeliveryPackageId)
             .OnDelete(DeleteBehavior.Cascade);

            b.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_DeliveryAccess_MaxDownloads_Positive",
                    "[MaxDownloads] IS NULL OR [MaxDownloads] > 0");
                tb.HasCheckConstraint("CK_DeliveryAccess_Downloads_NonNegative",
                    "[Downloads] >= 0");
            });
        }
    }
}