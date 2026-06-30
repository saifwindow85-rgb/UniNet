using Domain.Entities.Content;
using Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations.ContentConfigurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => p.PostId);
            builder.Property(p => p.PostId).ValueGeneratedOnAdd();

            builder.Property(p => p.Title).HasColumnType("NVARCHAR(500)").IsRequired();
            builder.Property(p => p.Content).HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(p => p.Type).HasConversion<byte>().HasColumnType("TINYINT").IsRequired();

            //Releation Image(1) => Post(1)
            builder.HasOne(p => p.Image).WithOne(i => i.Post)
                .IsRequired(false).HasForeignKey<Post>(p => p.ImageId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
