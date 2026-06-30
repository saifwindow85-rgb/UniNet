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
    public class UniversityConfiguration : IEntityTypeConfiguration<University>
    {
        public void Configure(EntityTypeBuilder<University> builder)
        {
            builder.HasKey(u => u.UniversityId);
            builder.Property(u => u.UniversityId).ValueGeneratedOnAdd();

            builder.Property(u => u.Name).HasColumnType("NVARCHAR(250)").IsRequired();
            builder.HasIndex(u => u.Name).IsUnique();

            builder.Property(u => u.Description).HasColumnType("NVARCHAR(MAX)").IsRequired(false);

            builder.ToTable("Universities");

            builder.HasData(new University
            {
                UniversityId = 1,
                Name = "Hadhramout University",
                Description = "Hadhramout University (HU), established in 1993 in Mukalla, Yemen, stands as a  " +
                    "cornerstone of higher education and academic excellence in the region. Since " +
                    "its inception, the university has been dedicated to fostering intellectual   " +
                    "growth, scientific research, and community development. It offers a diverse  " +
                    "array of undergraduate and postgraduate programs across various disciplines, " +
                    "including Engineering, Petroleum, Medicine, Sciences, Arts, and Humanities.  " +
                    "                                                                            " +
                    "The university serves as a vital hub for students from across Hadhramout and " +
                    "beyond, striving to prepare them with the skills and knowledge required to   " +
                    "meet the demands of a changing global landscape. HU is committed to          " +
                    "maintaining high academic standards and encouraging innovation through its   " +
                    "research centers and laboratories. By integrating theoretical knowledge with " +
                    "practical application, Hadhramout University plays a pivotal role in shaping " +
                    "the future leaders and professionals of Yemen, contributing significantly to " +
                    "the social and economic progress of the nation.",
                CreatedAt = new DateTime(2026, 1, 1),
                CreatedByUserId = 1

            });
        }
    }
}
