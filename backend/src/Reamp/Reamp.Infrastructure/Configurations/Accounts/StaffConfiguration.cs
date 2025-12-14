using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reamp.Domain.Accounts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Configurations.Accounts
{
    public class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> b)
        {
            b.ToTable("Staffs");
            b.HasKey(x => x.Id);

            b.Property(x => x.UserProfileId).IsRequired();
            b.HasIndex(x => x.UserProfileId)
             .IsUnique()
             .HasFilter("[DeletedAtUtc] IS NULL");
            b.HasOne<UserProfile>().WithOne()
             .HasForeignKey<Staff>(x => x.UserProfileId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Property(x => x.StudioId).IsRequired();
            b.HasOne<Studio>().WithMany()
             .HasForeignKey(x => x.StudioId)
             .OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(x => x.StudioId);

            b.Property(x => x.Role).IsRequired().HasConversion<int>();
            b.Property(x => x.Skills).IsRequired().HasConversion<int>();

            b.HasQueryFilter(x => x.DeletedAtUtc == null);

            b.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Staffs_Role_Valid", "[Role] >= 1 AND [Role] <= 3");
                tb.HasCheckConstraint("CK_Staffs_Skills_Valid", "[Skills] >= 0");
            });
        }
    }
}