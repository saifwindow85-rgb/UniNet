using Domain.Entities.Study;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations.Study_Configuration
{
    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.HasKey(s => s.SubjectId);
            builder.Property(s => s.SubjectId).ValueGeneratedOnAdd();

            builder.Property(s => s.Code).HasColumnType("NVARCHAR(25)").IsRequired();
            builder.Property(s => s.Name).HasColumnType("NVARCHAR(200)").IsRequired();
            builder.Property(s => s.Description).HasColumnType("NVARCHAR(MAX)").IsRequired(false);

            builder.HasIndex(s => new { s.DepartmentId, s.Code }).IsUnique();
            builder.HasIndex(s => new { s.DepartmentId, s.Name }).IsUnique();

            //Releation : Department(1) => Subjects(*)
            builder.HasOne(s => s.Department).WithMany(d => d.Subjects)
                 .HasForeignKey(s => s.DepartmentId).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.ToTable("Subjects");

            builder.HasData(new Subject
            {
                SubjectId = 1,
                Code = "77777777777",
                Name = "Programming",
                CreditHours = 18,
                CreatedByUserId = 1,
                DepartmentId = 1,
                CreatedAt = new DateTime(2026, 06, 30),
            });
        }
    }
}
