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
    public class SemesterConfiguration : IEntityTypeConfiguration<Semester>
    {
        public void Configure(EntityTypeBuilder<Semester> builder)
        {
            builder.HasKey(s => s.SemesterId);
            builder.Property(s => s.SemesterId).ValueGeneratedOnAdd();

            builder.Property(s => s.Name).HasColumnType("NVARCHAR(200)").IsRequired();

            builder.Property(s => s.StartDate).IsRequired();
            builder.Property(s => s.EndDate).IsRequired();

            builder.Property(s => s.IsCurrent).IsRequired();

            builder.ToTable("Semesters");
            builder.HasData(new Semester
            {
                SemesterId = 1,
                Name = "Term1",
                StartDate = new DateTime(2026, 06, 30),
                EndDate = new DateTime(2026, 06, 30),
                IsCurrent = true,
                CreatedByUserId = 1
            });
        }
    }
}
