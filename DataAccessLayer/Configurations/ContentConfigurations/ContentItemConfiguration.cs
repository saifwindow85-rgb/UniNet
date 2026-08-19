using Domain.Entities.Content;
using Domain.Entities.Enums;
using Domain.Entities.Images;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations.ContentConfigurations
{
    public class ContentItemConfiguration : IEntityTypeConfiguration<ContentItem>
    {
        public void Configure(EntityTypeBuilder<ContentItem> builder)
        {
            builder.HasKey(c => c.ContentItemId);
            builder.Property(c => c.ContentItemId).ValueGeneratedOnAdd();

            builder.Property(c => c.Title).HasColumnType("NVARCHAR(500)").IsRequired();
            builder.Property(c => c.Body).HasColumnType("NVARCHAR(MAX)").IsRequired();

            builder.HasDiscriminator(c => c.Type)
                .HasValue<Post>(EncontentType.Post)
                .HasValue<Announcement>(EncontentType.Announcement);

            builder.Property(c => c.Scope).HasConversion<byte>().HasColumnType("TINYINT").IsRequired();

            builder.HasOne(c => c.Image)
                     .WithOne(i => i.ContentItem)
                     .HasForeignKey<Image>(i => i.ContentItemId)
                     .IsRequired(false)
                     .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("ContentItems");
        }
    }
}
