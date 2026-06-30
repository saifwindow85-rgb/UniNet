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
    public class LecturerConfiguration : IEntityTypeConfiguration<Lecturer>
    {
        public void Configure(EntityTypeBuilder<Lecturer> builder)
        {
            builder.HasKey(l => l.LecturerId);
            builder.Property(l => l.LecturerId).ValueGeneratedOnAdd();

            builder.Property(l => l.FullName).HasColumnType("NVARCHAR(250)").IsRequired();
            builder.Property(l => l.Email).HasColumnType("NVARCHAR(256)").IsRequired(false);
            builder.Property(l => l.PhoneNumber).HasColumnType("NVARCHAR(25)").IsRequired(false);

            builder.HasIndex(l => l.Email).IsUnique().HasFilter("[Email] IS NOT NULL");

            builder.HasData(new Lecturer
            {
                LecturerId = 1,
                FullName = "Amen Salim Alsaeed",
                Email = "amen@gmail.com",
                PhoneNumber = "+967 774569774"
            });
        }
    }
}
