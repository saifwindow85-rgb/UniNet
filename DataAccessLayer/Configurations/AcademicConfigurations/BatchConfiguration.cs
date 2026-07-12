using DataAccessLayer.Seeds;
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
    public class BatchConfiguration : IEntityTypeConfiguration<Batch>
    {
        public void Configure(EntityTypeBuilder<Batch> builder)
        {
            builder.HasKey(b => b.BatchId);
            builder.Property(b => b.BatchId).ValueGeneratedOnAdd();

            builder.Property(b => b.Name).HasColumnType("NVARCHAR(250)").IsRequired();
            builder.HasIndex(b => new {b.DepartmentId,b.Name}).IsUnique();

            builder.Property(b => b.Description).HasColumnType("NVARCHAR(MAX)").IsRequired(false);

            builder.Property(b => b.BatchYear).IsRequired();

            //Releation : One Department => Many Batches
            builder.HasOne(b => b.Department).WithMany(d => d.Batches)
                .HasForeignKey(b => b.DepartmentId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Batches");

            builder.HasData(SeedData.GetBatches());
        }
    }
}
