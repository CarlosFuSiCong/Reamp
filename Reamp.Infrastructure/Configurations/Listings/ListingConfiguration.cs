using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reamp.Domain.Listings.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
    public void Configure(EntityTypeBuilder<Listing> b)
    {
        b.ToTable("Listings", t => t.HasCheckConstraint("CK_Listings_Price_Positive", "[Price] > 0"));
        b.HasKey(x => x.Id);

        b.Property(x => x.Title).IsRequired().HasMaxLength(160);
        b.Property(x => x.Description).IsRequired().HasMaxLength(4000);
        b.Property(x => x.Price).HasColumnType("decimal(18,2)");
        b.Property(x => x.Currency).IsRequired().HasMaxLength(3);

        b.Property(x => x.Bedrooms);
        b.Property(x => x.Bathrooms);
        b.Property(x => x.ParkingSpaces);
        b.Property(x => x.FloorAreaSqm);
        b.Property(x => x.LandAreaSqm);

        b.Property(x => x.ListingType).HasConversion<int>().IsRequired();
        b.Property(x => x.PropertyType).HasConversion<int>().IsRequired();
        b.Property(x => x.Status).HasConversion<int>().IsRequired();

        b.OwnsOne(x => x.Address, a =>
        {
            a.Property(p => p.Line1).IsRequired().HasMaxLength(160);
            a.Property(p => p.Line2).HasMaxLength(160);
            a.Property(p => p.City).IsRequired().HasMaxLength(80);
            a.Property(p => p.State).IsRequired().HasMaxLength(80);
            a.Property(p => p.Postcode).IsRequired().HasMaxLength(20);
            a.Property(p => p.Country).IsRequired().HasMaxLength(2);
            a.Property(p => p.Latitude);
            a.Property(p => p.Longitude);
        });

        b.HasIndex(x => new { x.Status, x.ListingType });
        b.HasIndex(x => x.PropertyType);
        b.HasIndex(x => x.Price);
    }
}
