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

            builder.HasData(new Department
            {
                CollegeId = 1,
                DepartmentId = 1,
                Name = "IT Department",
                Description = "The Department of Information Technology focuses on the intersection of       " +
                 "computing, infrastructure, and human-centric design. We equip students with  " +
                 "the core competencies in network administration, database management, web    " +
                 "development, and system security. The department emphasizes a hands-on       " +
                 "approach to learning, ensuring that graduates are proficient in managing     " +
                 "modern enterprise environments and digital workflows.                        " +
                 "                                                                             " +
                 "Through our rigorous training and focus on emerging technologies, we prepare " +
                 "our students to become the architects of tomorrow's infrastructure. We are   " +
                 "committed to maintaining high technical standards, fostering industry-ready  " +
                 "skills, and encouraging the strategic application of technology to solve     " +
                 "organizational challenges in an increasingly interconnected global market.",
                CreatedAt = new DateTime(2026, 1, 1),
                CreatedByUserId = 1

            });
        }
    }
}
