using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reamp.Domain.Listings.Entities;
using Reamp.Domain.Media.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ListingMediaRefConfiguration : IEntityTypeConfiguration<ListingMediaRef>
{
    public void Configure(EntityTypeBuilder<ListingMediaRef> b)
    {
        b.ToTable("ListingMediaRefs");
        b.HasKey(x => x.Id);

        b.Property(x => x.Role).HasConversion<int>().IsRequired();
        b.Property(x => x.SortOrder).HasDefaultValue(0);
        b.Property(x => x.IsCover).HasDefaultValue(false);

        b.HasIndex(x => x.ListingId);
        b.HasOne<Listing>()
            .WithMany(l => l.MediaRefs)
            .HasForeignKey(x => x.ListingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Do NOT cascade delete media asset
        b.HasIndex(x => x.MediaAssetId);
        b.HasOne<MediaAsset>()
            .WithMany()
            .HasForeignKey(x => x.MediaAssetId)
            .OnDelete(DeleteBehavior.NoAction);

        b.HasIndex(x => new { x.ListingId, x.IsCover })
         .HasFilter("[IsCover] = 1")
         .IsUnique();

        b.HasIndex(x => new { x.ListingId, x.Role, x.SortOrder }).IsUnique();
    }
}