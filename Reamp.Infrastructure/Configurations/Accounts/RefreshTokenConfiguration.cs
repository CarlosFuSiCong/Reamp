using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reamp.Domain.Accounts.Entities;

namespace Reamp.Infrastructure.Configurations.Accounts
{
    public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.UserId)
                .IsRequired();

            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(rt => rt.ExpiresAt)
                .IsRequired();

            builder.Property(rt => rt.CreatedAt)
                .IsRequired();

            builder.Property(rt => rt.IsRevoked)
                .IsRequired();

            builder.Property(rt => rt.RevokedAt);

            builder.Property(rt => rt.ReplacedByToken)
                .HasMaxLength(200);

            // Indexes
            builder.HasIndex(rt => rt.Token)
                .IsUnique();

            builder.HasIndex(rt => rt.UserId);

            builder.HasIndex(rt => new { rt.UserId, rt.IsRevoked, rt.ExpiresAt });
        }
    }
}

