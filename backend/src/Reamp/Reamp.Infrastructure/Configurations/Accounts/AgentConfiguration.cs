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
    public class AgentConfiguration : IEntityTypeConfiguration<Agent>
    {
        public void Configure(EntityTypeBuilder<Agent> b)
        {
            b.ToTable("Agents");
            b.HasKey(x => x.Id);

            b.Property(x => x.UserProfileId).IsRequired();
            b.HasIndex(x => x.UserProfileId)
             .IsUnique()
             .HasFilter("[DeletedAtUtc] IS NULL");
            b.HasOne<UserProfile>().WithOne()
             .HasForeignKey<Agent>(x => x.UserProfileId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Property(x => x.AgencyId).IsRequired();
            b.HasOne<Agency>()
             .WithMany()
             .HasForeignKey(x => x.AgencyId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<AgencyBranch>()
             .WithMany()
             .HasForeignKey(x => x.AgencyBranchId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.AgencyId);
            b.HasIndex(x => x.AgencyBranchId);

            b.Property(x => x.Role).IsRequired().HasConversion<int>();

            b.HasQueryFilter(x => x.DeletedAtUtc == null);

            b.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Agents_Role_Valid", "[Role] >= 0 AND [Role] <= 3");
            });
        }
    }
}

