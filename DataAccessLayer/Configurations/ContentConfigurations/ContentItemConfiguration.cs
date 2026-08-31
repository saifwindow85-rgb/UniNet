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

            //Releation : ContentItem(1) => Image(1)
            builder.HasOne(c => c.Image)
                     .WithOne(i => i.ContentItem)
                     .HasForeignKey<Image>(i => i.ContentItemId)
                     .IsRequired()
                     .OnDelete(DeleteBehavior.Cascade);

            // ----------------------------------------------------------------------------
            // نطاق النشر — أربع علاقات اختيارية تمثّل سلسلة الأجداد.
            // WithMany() بلا ملاحية عكسية عن قصد: university.ContentItems مجموعة لن تُحمَّل أبدًا
            // (عشرات الآلاف من الصفوف)، وإضافتها تُلوّث كيانات الهيكل الأكاديمي بلا فائدة.
            // نفس ما يفعله ConfigureBaseEntity مع CreatedByUser.
            // Restrict في الأربع: لا مسارات حذف متتالية متعددة، ومحتوى الجامعة لا يختفي بحذفها صامتًا.
            // ----------------------------------------------------------------------------
            builder.HasOne(c => c.University).WithMany()
                .HasForeignKey(c => c.UniversityId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.College).WithMany()
                .HasForeignKey(c => c.CollegeId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Department).WithMany()
                .HasForeignKey(c => c.DepartmentId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Batch).WithMany()
                .HasForeignKey(c => c.BatchId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            // ----------------------------------------------------------------------------
            // القيد الذي يجعل التنزيل (denormalization) آمنًا:
            // يربط Scope بالأعمدة المسموح لها أن تكون NOT NULL، فيصير الصف المتناقض
            // (Scope = Batch بلا BatchId، أو Public ومعه CollegeId) مستحيل الإدراج —
            // لا اعتمادًا على انضباط طبقة الخدمة، بل على قاعدة البيانات نفسها.
            // القيم: Public=1, Batch=2, Department=3, College=4, University=5
            // ----------------------------------------------------------------------------
            builder.ToTable("ContentItems", t => t.HasCheckConstraint(
                "CK_ContentItems_ScopeTargets",
                "([Scope] = 1 AND [UniversityId] IS NULL AND [CollegeId] IS NULL AND [DepartmentId] IS NULL AND [BatchId] IS NULL)" +
                " OR ([Scope] = 5 AND [UniversityId] IS NOT NULL AND [CollegeId] IS NULL AND [DepartmentId] IS NULL AND [BatchId] IS NULL)" +
                " OR ([Scope] = 4 AND [UniversityId] IS NOT NULL AND [CollegeId] IS NOT NULL AND [DepartmentId] IS NULL AND [BatchId] IS NULL)" +
                " OR ([Scope] = 3 AND [UniversityId] IS NOT NULL AND [CollegeId] IS NOT NULL AND [DepartmentId] IS NOT NULL AND [BatchId] IS NULL)" +
                " OR ([Scope] = 2 AND [UniversityId] IS NOT NULL AND [CollegeId] IS NOT NULL AND [DepartmentId] IS NOT NULL AND [BatchId] IS NOT NULL)"));
        }
    }
}
