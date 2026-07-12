using Domain.Entities.Images;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations.Image_Configuration
{
    internal class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasKey(i => i.ImageId);
            builder.Property(i => i.ImageId).ValueGeneratedOnAdd();

            builder.Property(i => i.FileName).HasColumnType("NVARCHAR(250)").IsRequired();
            builder.Property(i => i.FilePath).HasColumnType("NVARCHAR(500)").IsRequired();

            builder.Property(i => i.UploadedAt).HasDefaultValueSql("GETUTCDATE()");

 

            //Relealtion : UploadedByUser(1) => Images(*)
            builder.HasOne(i => i.UploadedByUser).WithMany(u => u.UploadedImages)
                .HasForeignKey(i => i.UploadedByUserId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            //Releation : UpdatedByUser(1) => Images(*)
            builder.HasOne(i => i.UpdatedByUser).WithMany(u => u.UpdatedImages)
                .HasForeignKey(i => i.UpdatedByUserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Images");

        }
    }
}
