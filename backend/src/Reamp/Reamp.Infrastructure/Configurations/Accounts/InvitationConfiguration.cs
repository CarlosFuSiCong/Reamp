using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;

namespace Reamp.Infrastructure.Configurations.Accounts
{
    public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
    {
        public void Configure(EntityTypeBuilder<Invitation> b)
        {
            b.ToTable("Invitations");
            b.HasKey(x => x.Id);

            b.Property(x => x.Type)
                .IsRequired()
                .HasConversion<int>();

            b.Property(x => x.TargetEntityId)
                .IsRequired();

            b.Property(x => x.TargetBranchId)
                .IsRequired(false);

            b.Property(x => x.InviteeEmail)
                .IsRequired()
                .HasMaxLength(255);

            b.Property(x => x.InviteeUserId)
                .IsRequired(false);

            b.Property(x => x.TargetRoleValue)
                .IsRequired();

            b.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>();

            b.Property(x => x.InvitedBy)
                .IsRequired();

            b.HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(x => x.InvitedBy)
                .OnDelete(DeleteBehavior.Restrict);

            b.Property(x => x.ExpiresAtUtc)
                .IsRequired();

            b.Property(x => x.RespondedAtUtc)
                .IsRequired(false);

            b.HasIndex(x => x.InviteeEmail);
            b.HasIndex(x => new { x.TargetEntityId, x.Type });
            b.HasIndex(x => new { x.InviteeEmail, x.Status, x.ExpiresAtUtc });

            b.HasQueryFilter(x => x.DeletedAtUtc == null);

            b.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Invitations_Type_Valid", "[Type] >= 0 AND [Type] <= 1");
                tb.HasCheckConstraint("CK_Invitations_Status_Valid", "[Status] >= 0 AND [Status] <= 4");
            });
        }
    }
}
