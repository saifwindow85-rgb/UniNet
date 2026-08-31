using DataAccessLayer.Seeds;
using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations.Academic_Configurations.Identity_Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.RoleId);
            builder.Property(r => r.RoleId).ValueGeneratedOnAdd();

            builder.Property(r => r.Name).HasColumnType("NVARCHAR").HasMaxLength(250).IsRequired();
            builder.HasIndex(r => r.Name).IsUnique();

            builder.ToTable("Roles");

            builder.HasData(SeedData.GetRoles());
        }
    }
}
