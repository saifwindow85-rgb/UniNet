using DataAccessLayer.Seeds;
using Domain.Entities.Academic_Structure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations.Academic_Structure
{
    public class UniversityConfiguration : IEntityTypeConfiguration<University>
    {
        public void Configure(EntityTypeBuilder<University> builder)
        {
            builder.HasKey(u => u.UniversityId);
            builder.Property(u => u.UniversityId).ValueGeneratedOnAdd();

            builder.Property(u => u.Name).HasColumnType("NVARCHAR(250)").IsRequired();
            builder.HasIndex(u => u.Name).IsUnique();

            builder.Property(u => u.Description).HasColumnType("NVARCHAR(MAX)").IsRequired(false);

            builder.ToTable("Universities");

            builder.HasData(SeedData.GetUniversities());
        }
    }
}
