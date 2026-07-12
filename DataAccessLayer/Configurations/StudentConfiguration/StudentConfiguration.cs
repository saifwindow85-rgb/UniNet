using DataAccessLayer.Seeds;
using Domain.Entities.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations.Student_Configuration
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(s => s.StudentId);
            builder.Property(s => s.StudentId).ValueGeneratedOnAdd();

            builder.Property(s => s.StudentNumber).HasColumnType("NVARCHAR(20)").IsRequired();
            builder.Property(s => s.EnrollmentDate).HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(s => s.UserId).IsUnique();
            builder.HasIndex(s => s.StudentNumber).IsUnique();
            // Releation : Student(1) => User(1)
            builder.HasOne(s => s.User).WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.UserId).OnDelete(DeleteBehavior.Restrict);

            //Releation : Batch(1) => Students(*)
            builder.HasOne(s => s.Batch).WithMany(b => b.Students)
                .HasForeignKey(s => s.BatchId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            // Releation : Section(1) => Students(*)
            builder.HasOne(s => s.Section).WithMany(s => s.Students)
                .HasForeignKey(s => s.SectionId).IsRequired(false) // At The Begnning Student May Not Belong To Any Section
                .OnDelete(DeleteBehavior.Restrict);

            //Releation : Status(1) => Students(*)
            builder.HasOne(s => s.Status).WithMany(s => s.Students)
                .HasForeignKey(s => s.StatusId).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.ToTable("Students");

            builder.HasData(SeedData.GetStudents());
        }
    }
}
