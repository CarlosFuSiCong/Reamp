using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reamp.Domain.Accounts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> b)
        {
            b.ToTable("Clients");
            b.HasKey(x => x.Id);

            b.Property(x => x.UserProfileId).IsRequired();
            b.HasIndex(x => x.UserProfileId).IsUnique();
            b.HasOne<UserProfile>().WithOne()
             .HasForeignKey<Client>(x => x.UserProfileId)
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

            b.HasQueryFilter(x => x.DeletedAtUtc == null);
        }
    }
}