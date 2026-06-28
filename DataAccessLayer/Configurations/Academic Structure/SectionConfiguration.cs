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
    public class SectionConfiguration : IEntityTypeConfiguration<Section>
    {
        public void Configure(EntityTypeBuilder<Section> builder)
        {
            builder.HasKey(s => s.SectionId);
            builder.Property(s => s.SectionId).ValueGeneratedOnAdd();

            builder.Property(s => s.Name).HasColumnType("NVARCHAR(250)").IsRequired();
            builder.HasIndex(s => new { s.BatchId, s.Name }).IsUnique();
            
            //Releation : One Batch => Many Sections 
            builder.HasOne(s=>s.Batch).WithMany(b=>b.Sections)
                .HasForeignKey(s=>s.BatchId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.HasData(new Section
            {
                BatchId = 1,
                SectionId = 1,
                Name = "A",
            });
        }
    }
}
