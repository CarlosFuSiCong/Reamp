using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reamp.Domain.Media.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Configurations.Media
{
    internal class MediaVariantConfiguration : IEntityTypeConfiguration<MediaVariant>
    {
        public void Configure(EntityTypeBuilder<MediaVariant> b)
        {
            b.ToTable("MediaVariants", t =>
            {
                t.HasCheckConstraint("CK_MediaVariants_Size_Positive", "[SizeBytes] IS NULL OR [SizeBytes] > 0");
            });

            b.HasKey(x => x.Id);

            b.Property(x => x.Name).IsRequired().HasMaxLength(64);
            b.Property(x => x.Url).IsRequired().HasMaxLength(1024);
            b.Property(x => x.Format).HasMaxLength(32);
            b.Property(x => x.SortOrder).HasDefaultValue(0);

            b.HasIndex(x => x.MediaAssetId);

            // same asset + same variant name must be unique
            b.HasIndex(x => new { x.MediaAssetId, x.Name }).IsUnique();
        }
    }
}