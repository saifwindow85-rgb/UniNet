using Domain.Entities.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations.Employees_Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.EmployeeId);
            builder.Property(e => e.EmployeeId).ValueGeneratedOnAdd();

            builder.HasIndex(e => e.UserId).IsUnique();
            
            //Releation : University(1) => Employees(*)
            builder.HasOne(e=>e.University).WithMany(u=>u.Employees)
                .HasForeignKey(e=>e.UniversityId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            //Releation : College(1) => Employees(*)
            builder.HasOne(e=>e.College).WithMany(c=>c.Employees)
                .HasForeignKey(e=>e.CollegeId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            //Releation : Department(1) => Employees(*)
            builder.HasOne(e => e.Department).WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            builder.HasData(new Employee
            {
                EmployeeId = 1,
                UserId = 1,
                UniversityId = 1,
                CollegeId = 1,
                DepartmentId = 1,
            });
        }
    }
}
