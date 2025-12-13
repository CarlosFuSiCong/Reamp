using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reamp.Domain.Accounts.Entities;

namespace Reamp.Infrastructure.Configurations.Accounts
{
    public class OrganizationApplicationConfiguration : IEntityTypeConfiguration<OrganizationApplication>
    {
        public void Configure(EntityTypeBuilder<OrganizationApplication> b)
        {
            b.ToTable("OrganizationApplications");
            b.HasKey(x => x.Id);

            b.Property(x => x.Type)
                .IsRequired()
                .HasConversion<int>();

            b.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>();

            b.Property(x => x.ApplicantUserId)
                .IsRequired();

            // Note: ApplicantUserId stores ApplicationUser.Id, not UserProfile.Id
            // No foreign key constraint to allow flexibility

            b.Property(x => x.OrganizationName)
                .IsRequired()
                .HasMaxLength(100);

            b.Property(x => x.Description)
                .IsRequired(false)
                .HasMaxLength(500);

            b.Property(x => x.ContactEmail)
                .IsRequired()
                .HasMaxLength(255);

            b.Property(x => x.ContactPhone)
                .IsRequired()
                .HasMaxLength(50);

            b.OwnsOne(x => x.Address, addr =>
            {
                addr.Property(a => a.Line1).HasMaxLength(200).HasColumnName("Address_Line1");
                addr.Property(a => a.Line2).HasMaxLength(200).HasColumnName("Address_Line2");
                addr.Property(a => a.City).HasMaxLength(100).HasColumnName("Address_City");
                addr.Property(a => a.State).HasMaxLength(100).HasColumnName("Address_State");
                addr.Property(a => a.Postcode).HasMaxLength(20).HasColumnName("Address_Postcode");
                addr.Property(a => a.Country).HasMaxLength(2).HasColumnName("Address_Country");
            });

            b.Property(x => x.CreatedOrganizationId)
                .IsRequired(false);

            b.Property(x => x.ReviewedBy)
                .IsRequired(false);

            b.Property(x => x.ReviewedAtUtc)
                .IsRequired(false);

            b.Property(x => x.ReviewNotes)
                .IsRequired(false)
                .HasMaxLength(500);

            b.HasIndex(x => x.ApplicantUserId);
            b.HasIndex(x => new { x.Status, x.Type });
            b.HasIndex(x => x.CreatedAtUtc);

            b.HasQueryFilter(x => x.DeletedAtUtc == null);

            b.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_OrganizationApplications_Type_Valid", "[Type] >= 0 AND [Type] <= 1");
                tb.HasCheckConstraint("CK_OrganizationApplications_Status_Valid", "[Status] >= 0 AND [Status] <= 4");
            });
        }
    }
}
