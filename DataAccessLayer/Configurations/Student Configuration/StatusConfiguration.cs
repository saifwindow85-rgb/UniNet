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
    public class StatusConfiguration : IEntityTypeConfiguration<StudentStatus>
    {
        public void Configure(EntityTypeBuilder<StudentStatus> builder)
        {
            builder.HasKey(s => s.StatusId);
            builder.Property(s => s.StatusId).ValueGeneratedOnAdd();

            builder.Property(s => s.Name).HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(s => s.Description).HasColumnType("NVARCHAR(300)").IsRequired(false);

            builder.HasIndex(s => s.Name).IsUnique();
            builder.ToTable("StudentStatuses");

            builder.HasData(SeedData.GetStudentStatuses());
        }
    }
}
