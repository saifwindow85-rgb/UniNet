using DataAccessLayer.Seeds;
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
    public class StudentResultConfiguration : IEntityTypeConfiguration<StudentResult>
    {
        public void Configure(EntityTypeBuilder<StudentResult> builder)
        {
            builder.HasKey(s => s.StudentResultId);
            builder.Property(s => s.StudentResultId).ValueGeneratedOnAdd();

            builder.Property(s => s.CreatedAt).HasDefaultValueSql("GETUTCDATE()").IsRequired();

            builder.Property(s => s.Midterm).HasPrecision(5, 2);
            builder.Property(s => s.Practical).HasPrecision(5, 2);
            builder.Property(s => s.Final).HasPrecision(5, 2);
            builder.Property(s => s.Total).HasPrecision(5, 2).HasComputedColumnSql("[Midterm] + [Practical] + [Final]", stored: true);

            // Releation : Student(1) => StudentResults(*)
            builder.HasOne(s => s.Student).WithMany(s => s.StudentResults)
                .HasForeignKey(s => s.StudentId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            //Releation : SectionSubject(1) => StudentResults(*)
            builder.HasOne(s => s.SectionSubject).WithMany(s => s.StudentResults)
                .HasForeignKey(s => s.SectionSubjectId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            // EnterdByUser(1) => StudentResults(*)
            builder.HasOne(s => s.EnterdByUser).WithMany(e => e.EnteredByStudentResults)
                .HasForeignKey(s => s.EnteredByUserId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.UpdatedByUser).WithMany(e => e.UpdatedStudentResults)
                .HasForeignKey(s => s.UpdatedByUserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            builder.ToTable("StudentResults");

            builder.HasData(SeedData.GetStudentResults());
        }
    }
}
