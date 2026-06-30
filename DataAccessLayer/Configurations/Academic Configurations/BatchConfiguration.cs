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

            builder.HasData(new Batch
            {
                DepartmentId = 1,
                BatchId = 1,
                BatchYear = 2025,
                Name = "Kernel",
                Description = "In computer science, a Batch (or Kernel-level batching) refers to the         " +
                 "processing of a collection of tasks or data sets in a single sequence without " +
                 "manual intervention. By grouping operations, the system minimizes overhead   " +
                 "and maximizes throughput, allowing the kernel to handle resource allocation  " +
                 "more efficiently.                                                            " +
                 "                                                                             " +
                 "This approach is fundamental to modern computing, optimizing CPU cycles and  " +
                 "memory management by reducing the frequency of context switching. Whether    " +
                 "applied in system-level kernel processes or data-intensive application tasks," +
                 "the Batch model ensures consistent, high-performance execution of repetitive " +
                 "operations, critical for building scalable and robust software architectures.",
                CreatedAt = new DateTime(2026, 1, 1)
            });
        }
    }
}
