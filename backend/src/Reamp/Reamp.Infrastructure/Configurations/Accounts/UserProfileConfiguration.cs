using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Media.Entities;
using Reamp.Infrastructure.Identity;

namespace Reamp.Infrastructure.Configurations.Accounts
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> b)
        {
            b.ToTable("UserProfiles");
            b.HasKey(x => x.Id);

            b.HasIndex(x => x.ApplicationUserId)
             .IsUnique()
             .HasFilter("[DeletedAtUtc] IS NULL");

            b.HasOne<ApplicationUser>()
             .WithOne()
             .HasForeignKey<UserProfile>(x => x.ApplicationUserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Property(x => x.ApplicationUserId).IsRequired();
            b.Property(x => x.FirstName).IsRequired().HasMaxLength(40);
            b.Property(x => x.LastName).IsRequired(false).HasMaxLength(40);

            b.Property(x => x.AvatarAssetId).IsRequired(false);
            b.HasOne<MediaAsset>()
             .WithMany()
             .HasForeignKey(x => x.AvatarAssetId)
             .OnDelete(DeleteBehavior.NoAction);

            b.Property(x => x.Role)
             .IsRequired()
             .HasConversion<int>();

            b.Property(x => x.Status)
             .IsRequired()
             .HasConversion<int>();

            b.HasQueryFilter(x => x.DeletedAtUtc == null);

            b.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_UserProfiles_FirstName_NotEmpty",
                    "LEN(LTRIM(RTRIM([FirstName]))) > 0");
            });
        }
    }
}