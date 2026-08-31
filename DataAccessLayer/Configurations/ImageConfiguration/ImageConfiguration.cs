using Domain.Entities.Content;
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

            builder.Property(i => i.OriginalFileName).HasColumnType("NVARCHAR(250)").IsRequired();
            builder.Property(i => i.StoredFileName).HasColumnType("NVARCHAR(100)").IsRequired();
            builder.Property(i => i.RelativePath).HasColumnType("NVARCHAR(400)").IsRequired();
            builder.Property(i => i.ContentType).HasColumnType("NVARCHAR(100)").IsRequired();

            // علاقة ContentItem↔Image مُعرَّفة في ContentItemConfiguration — لا تُكرَّر هنا
            // وحقول التدقيق يتولاها ConfigureBaseEntity تلقائياً
            builder.ToTable("Images");
        }
    }
}
