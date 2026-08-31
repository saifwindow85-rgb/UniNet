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
    public class UserRoleConfigurations : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasKey(ur => new
            {
                ur.UserId,
                ur.RoleId
            });
            builder.HasIndex(u => new { u.UserId, u.RoleId }).IsUnique();


            //Releation : User(1) => (*)UserRole
            builder.HasOne(u => u.User).WithMany(u => u.UserRoles)
                .HasForeignKey(u => u.UserId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            // Releation : Role(1) => UserRole(*)
            builder.HasOne(u=>u.Role).WithMany(r=>r.UserRoles)
                .HasForeignKey(u=>u.RoleId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("UserRoles");

            builder.HasData(SeedData.GetUserRoles());
        }
    }
}
