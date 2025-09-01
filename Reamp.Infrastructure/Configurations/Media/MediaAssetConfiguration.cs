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
    internal class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
    {
        public void Configure(EntityTypeBuilder<MediaAsset> b)
        {
            b.ToTable("MediaAssets", t =>
            {
                t.HasCheckConstraint("CK_MediaAssets_Size_Positive", "[SizeBytes] > 0");
                t.HasCheckConstraint("CK_MediaAssets_Duration_Positive", "[DurationSeconds] IS NULL OR [DurationSeconds] > 0");
            });

            b.HasKey(x => x.Id);

            b.Property(x => x.Provider).HasConversion<int>().IsRequired();
            b.Property(x => x.ResourceType).HasConversion<int>().IsRequired();
            b.Property(x => x.ProcessStatus).HasConversion<int>().IsRequired();

            b.Property(x => x.ProviderAssetId).IsRequired().HasMaxLength(256);
            b.Property(x => x.ContentType).IsRequired().HasMaxLength(128);
            b.Property(x => x.OriginalFileName).HasMaxLength(256);
            b.Property(x => x.PublicUrl).HasMaxLength(1024);
            b.Property(x => x.ChecksumSha256).HasMaxLength(64);

            // unique per provider asset
            b.HasIndex(x => new { x.Provider, x.ProviderAssetId }).IsUnique();

            // deduplication index
            b.HasIndex(x => new { x.OwnerStudioId, x.ChecksumSha256 })
             .HasFilter("[ChecksumSha256] IS NOT NULL")
             .IsUnique();

            b.HasIndex(x => x.OwnerStudioId);
            b.HasIndex(x => x.ProcessStatus);

            b.HasMany(x => x.Variants)
             .WithOne()
             .HasForeignKey(v => v.MediaAssetId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}