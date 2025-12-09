using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Common.ValueObjects;
using Reamp.Domain.Media.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Configurations.Accounts
{
    public class AgencyConfiguration : IEntityTypeConfiguration<Agency>
    {
        public void Configure(EntityTypeBuilder<Agency> b)
        {
            b.ToTable("Agencies");
            b.HasKey(x => x.Id);

            b.Property(x => x.Name).IsRequired().HasMaxLength(120);
            b.Property(x => x.Description).HasMaxLength(512);

            b.Property(x => x.LogoAssetId).IsRequired(false);
            b.HasOne<MediaAsset>()
             .WithMany()
             .HasForeignKey(x => x.LogoAssetId)
             .OnDelete(DeleteBehavior.NoAction);

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

            b.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Agencies_ContactEmail_NotEmpty",
                    "LEN(LTRIM(RTRIM([ContactEmail]))) > 0");
                tb.HasCheckConstraint("CK_Agencies_ContactPhone_NotEmpty",
                    "LEN(LTRIM(RTRIM([ContactPhone]))) > 0");
            });
        }
    }
}