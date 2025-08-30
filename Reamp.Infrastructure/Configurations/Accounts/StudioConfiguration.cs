using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Configurations.Accounts
{
    public class StudioConfiguration : IEntityTypeConfiguration<Studio>
    {
        public void Configure(EntityTypeBuilder<Studio> b)
        {
            b.ToTable("Studios");
            b.HasKey(x => x.Id);

            b.Property(x => x.Name).IsRequired().HasMaxLength(120);
            b.Property(x => x.Description).HasMaxLength(512);
            b.Property(x => x.LogoUrl).HasMaxLength(256);
            b.Property(x => x.CreatedBy).IsRequired();
            b.Property(x => x.ContactEmail).IsRequired().HasMaxLength(120);
            b.Property(x => x.ContactPhone).IsRequired().HasMaxLength(40);

            b.HasQueryFilter(x => x.DeletedAtUtc == null);

            b.Property(x => x.Slug)
             .HasConversion(v => v.Value, v => Slug.From(v))
             .IsRequired()
             .HasMaxLength(140);

            b.HasIndex(x => x.Slug)
             .IsUnique()
             .HasFilter("[DeletedAtUtc] IS NULL");

            b.HasIndex(x => x.CreatedBy);

            b.OwnsOne(x => x.Address, a =>
            {
                a.Property(p => p.Line1).IsRequired().HasMaxLength(120).HasColumnName("Address_Line1");
                a.Property(p => p.Line2).HasMaxLength(120).HasColumnName("Address_Line2");
                a.Property(p => p.City).IsRequired().HasMaxLength(80).HasColumnName("Address_City");
                a.Property(p => p.State).IsRequired().HasMaxLength(40).HasColumnName("Address_State");
                a.Property(p => p.Postcode).IsRequired().HasMaxLength(10).HasColumnName("Address_Postcode");
                a.Property(p => p.Country).IsRequired().HasMaxLength(2).HasColumnName("Address_Country");
                a.Property(p => p.Latitude).HasPrecision(9, 6).HasColumnName("Address_Latitude");
                a.Property(p => p.Longitude).HasPrecision(9, 6).HasColumnName("Address_Longitude");
                a.HasIndex(p => p.City);
            });

            b.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Studios_ContactEmail_NotEmpty",
                    "LEN(LTRIM(RTRIM([ContactEmail]))) > 0");
                tb.HasCheckConstraint("CK_Studios_ContactPhone_NotEmpty",
                    "LEN(LTRIM(RTRIM([ContactPhone]))) > 0");
                tb.HasCheckConstraint("CK_Studios_Address_Lat",
                    "[Address_Latitude] IS NULL OR ([Address_Latitude] BETWEEN -90 AND 90)");
                tb.HasCheckConstraint("CK_Studios_Address_Lng",
                    "[Address_Longitude] IS NULL OR ([Address_Longitude] BETWEEN -180 AND 180)");
            });
        }
    }
}