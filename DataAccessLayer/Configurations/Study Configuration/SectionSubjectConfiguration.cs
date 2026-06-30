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
    public class SectionSubjectConfiguration : IEntityTypeConfiguration<SectionSubject>
    {
        public void Configure(EntityTypeBuilder<SectionSubject> builder)
        {
            builder.HasKey(s => s.SectionSubjectId);
            builder.Property(s => s.SectionSubjectId).ValueGeneratedOnAdd();

            //Releation : Section(1) => SectionSubjects(*)
            builder.HasOne(s => s.Section).WithMany(s => s.SectionSubjects)
                .HasForeignKey(s => s.SectionId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            //Releation Subject(1) => SectionSubjects(*)
            builder.HasOne(s=>s.Subject).WithMany(s=>s.SectionSubjects)
                .HasForeignKey(s=>s.SubjectId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            // Semester => SectionSubjects(*)
            builder.HasOne(s => s.Semester).WithMany(s => s.SectionSubjects)
              .HasForeignKey(s => s.SemesterId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            // Lecturer(1) => SectionSubjects(*)
            builder.HasOne(s => s.Lecturer).WithMany(s => s.SectionSubjects)
              .HasForeignKey(s => s.LecturerId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("SectionSubjects");
        }
    }
}
