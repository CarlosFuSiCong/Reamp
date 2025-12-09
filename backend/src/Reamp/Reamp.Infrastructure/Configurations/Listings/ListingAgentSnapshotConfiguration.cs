using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reamp.Domain.Listings.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Configurations.Listings
{
    public class ListingAgentSnapshotConfiguration : IEntityTypeConfiguration<ListingAgentSnapshot>
    {
        public void Configure(EntityTypeBuilder<ListingAgentSnapshot> b)
        {
            b.ToTable("ListingAgentSnapshots");
            b.HasKey(x => x.Id);

            b.Property(x => x.FirstName).IsRequired().HasMaxLength(80);
            b.Property(x => x.LastName).IsRequired().HasMaxLength(80);
            b.Property(x => x.Email).IsRequired().HasMaxLength(160);
            b.Property(x => x.PhoneNumber).HasMaxLength(40);
            b.Property(x => x.TeamOrOfficeName).HasMaxLength(160);
            b.Property(x => x.AvatarUrl).HasMaxLength(256);

            b.Property(x => x.IsPrimary).HasDefaultValue(false);
            b.Property(x => x.SortOrder).HasDefaultValue(0);

            b.HasIndex(x => x.ListingId);
            b.HasOne<Listing>()
                .WithMany(l => l.AgentSnapshots)
                .HasForeignKey(x => x.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.ListingId, x.IsPrimary })
             .HasFilter("[IsPrimary] = 1")
             .IsUnique();
        }
    }
}