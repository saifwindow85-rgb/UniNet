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
    public class CollegeConfiguration : IEntityTypeConfiguration<College>
    {
        public void Configure(EntityTypeBuilder<College> builder)
        {
            builder.HasKey(c => c.CollegeId);
            builder.Property(c => c.CollegeId).ValueGeneratedOnAdd();

            builder.Property(c => c.Name).HasColumnType("NVARCHAR(250)").IsRequired();
            builder.HasIndex(c => new { c.UniversityId, c.Name }).IsUnique();

            builder.Property(c => c.Description).HasColumnType("NVARCHAR(MAX)").IsRequired(false);

            //Releation : One University => Many Colleges
            builder.HasOne(c=>c.University).WithMany(u=>u.Colleges)
                .HasForeignKey(c=>c.UniversityId) .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Colleges");

            builder.HasData(SeedData.GetColleges());
        }
    }
}
