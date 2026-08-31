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
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(d => d.DepartmentId);
            builder.Property(d => d.DepartmentId).ValueGeneratedOnAdd();

            builder.Property(d => d.Name).HasColumnType("NVARCHAR(250)").IsRequired();
            builder.HasIndex(d => new {d.CollegeId, d.Name}).IsUnique();

            builder.Property(d => d.Description).HasColumnType("NVARCHAR(MAX)").IsRequired(false);

            //Releation : One College => Many Departments
            builder.HasOne(d => d.College).WithMany(c => c.Departments)
                .HasForeignKey(d => d.CollegeId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Departments");

            builder.HasData(SeedData.GetDepartments());
        }
    }
}
