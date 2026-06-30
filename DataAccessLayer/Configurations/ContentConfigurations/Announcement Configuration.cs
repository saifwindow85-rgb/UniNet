using Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations.ContentConfigurations
{
    public class Announcement_Configuration : IEntityTypeConfiguration<Announcement>
    {
        public void Configure(EntityTypeBuilder<Announcement> builder)
        {
            builder.HasKey(a => a.AnnouncementId);
            builder.Property(a => a.AnnouncementId).ValueGeneratedOnAdd();

            builder.Property(a => a.Title).HasColumnType("NVARCHAR(500)").IsRequired();
            builder.Property(a => a.Content).HasColumnType("NVARCHAR(MAX)").IsRequired();
            builder.Property(a => a.Type).HasConversion<byte>().HasColumnType("TINYINT").IsRequired();

            //Releation Image(1) => Post(1)
            builder.HasOne(a => a.Image).WithOne(i => i.Announcement)
                .IsRequired(false).HasForeignKey<Announcement>(p => p.ImageId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
