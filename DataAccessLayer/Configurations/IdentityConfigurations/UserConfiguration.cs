using DataAccessLayer.Seeds;
using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations.Identity_Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.UserId);
            builder.Property(u => u.UserId).ValueGeneratedOnAdd();

            builder.Property(u => u.FullName).HasColumnType("NVARCHAR(250)").IsRequired();//No Unique Constraint For Name
            builder.Property(u => u.UserName).HasColumnType("NVARCHAR(250)").IsRequired();

            builder.Property(r => r.PasswordHash).HasColumnType("NVARCHAR(MAX)").IsRequired();

            builder.Property(u => u.Email).HasColumnType("NVARCHAR(256)").IsRequired(false);
            builder.Property(u => u.PhoneNumber).HasColumnType("NVARCHAR(25)").IsRequired(false);

            builder.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(u => u.UserName).IsUnique();
            builder.HasIndex(r => r.Email).IsUnique().HasFilter("[Email] IS NOT NULL");

            //Releation : one User Created by One User & Many Users Created By One User
            builder.HasOne(u => u.CreatedByUser).WithMany(u => u.CreatedUsers)
                .HasForeignKey(u => u.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
            //Releation : one User Updated by One User & Many Users Updated By One User
            builder.HasOne(u => u.UpdatedByUser).WithMany(u => u.UpdatedUsers)
                .HasForeignKey(u => u.UpdatedByUserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Users");

            builder.HasData(SeedData.GetUsers());
        }

    }
}
