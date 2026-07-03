using Domain.Entities.Token;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations.Tokens
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(r => r.RefreshTokenId);

            builder.Property(r => r.TokenHash)
                   .HasMaxLength(256)
                   .IsRequired();

            builder.Property(r => r.ExpiresAt)
                   .IsRequired();

            builder.HasIndex(r => r.TokenHash)
                   .IsUnique();

            builder.HasOne(r => r.User)
                   .WithMany(u => u.RefreshTokens)
                   .HasForeignKey(r => r.UserId)
                   .IsRequired();

            builder.ToTable("RefreshTokens");
        }
    }
}
