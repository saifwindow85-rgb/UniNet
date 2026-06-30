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
    public class ColleageConfiguration : IEntityTypeConfiguration<College>
    {
        public void Configure(EntityTypeBuilder<College> builder)
        {
            builder.HasKey(c => c.CollegeId);
            builder.Property(c => c.CollegeId).ValueGeneratedOnAdd();

            builder.Property(c => c.Name).HasColumnType("NVARCHAR(250)").IsRequired();
            builder.HasIndex(c => new { c.UniversityId, c.Name }).IsUnique();

            builder.Property(c => c.Description).HasColumnType("NVARCHAR(MAX)").IsRequired(false);

            //Releation : One University => Many Colleges
            builder.HasOne(c=>c.University).WithMany(u=>u.Colleges)
                .HasForeignKey(c=>c.UniversityId) .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(new College
            {
                CollegeId = 1,
                UniversityId = 1,
                Name = "The Faculty of Computer Science and Information Technology",
                Description = "The Faculty of Computer Science and Information Technology is a center for    " +
                     "innovation and academic excellence, dedicated to preparing students for the   " +
                     "digital era. The curriculum covers essential fields such as Software         " +
                     "Engineering, Artificial Intelligence, Cybersecurity, and Data Science.       " +
                     "                                                                             " +
                     "By bridging the gap between theoretical computing principles and real-world  " +
                     "applications, the faculty empowers students with the analytical and          " +
                     "technical skills needed to solve complex problems. Through state-of-the-art  " +
                     "labs, collaborative research projects, and partnerships with the tech        " +
                     "industry, we foster a creative environment. Our goal is to graduate leaders  " +
                     "capable of driving technological progress and contributing significantly to  " +
                     "the advancement of the national and international digital economy.",


            });
        }
    }
}
