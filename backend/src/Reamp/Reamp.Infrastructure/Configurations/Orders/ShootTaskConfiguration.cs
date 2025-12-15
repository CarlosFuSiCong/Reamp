using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reamp.Domain.Shoots.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Infrastructure.Configurations.Shoots
{
    public class ShootTaskConfiguration : IEntityTypeConfiguration<ShootTask>
    {
        public void Configure(EntityTypeBuilder<ShootTask> b)
        {
            b.ToTable("ShootTasks", t =>
            {
                t.HasCheckConstraint("CK_ShootTasks_Time",
                    "[ScheduledStartUtc] IS NULL OR [ScheduledEndUtc] IS NULL OR [ScheduledEndUtc] > [ScheduledStartUtc]");
                t.HasCheckConstraint("CK_ShootTasks_Price_Positive",
                    "[Price] IS NULL OR [Price] > 0");
                t.HasCheckConstraint("CK_ShootTasks_Type_NonNegative", "[Type] >= 0"); // flags enum stored as int
            });

            b.HasKey(x => x.Id);

            b.Property(x => x.ShootOrderId).IsRequired();
            b.Property(x => x.Type).HasConversion<int>().IsRequired();
            b.Property(x => x.Status).HasConversion<int>().IsRequired();
            b.Property(x => x.AssigneeUserId).IsRequired(false); // Optional assignee

            b.Property(x => x.Price).HasColumnType("decimal(18,2)");
            b.Property(x => x.Notes).HasMaxLength(4000);

            b.HasIndex(x => x.ShootOrderId);
            b.HasIndex(x => new { x.Type, x.Status });

            b.HasOne<ShootOrder>()
             .WithMany(o => (ICollection<ShootTask>)o.Tasks)
             .HasForeignKey(x => x.ShootOrderId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}