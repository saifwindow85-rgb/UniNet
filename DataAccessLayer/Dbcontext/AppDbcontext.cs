using Domain.Entities.Academic_Structure;
using Domain.Entities.Common;
using Domain.Entities.Content;
using Domain.Entities.Employees;
using Domain.Entities.Identity;
using Domain.Entities.Images;
using Domain.Entities.Students;
using Domain.Entities.Study;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dbcontext
{
    public class AppDbcontext : DbContext
    {
        //Identity
        public DbSet<Role> Roles {  get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        //Academic Structure
        public DbSet<University> Universities { get; set; }
        public DbSet<College> Colleges { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Section>Sections { get; set; }
        //Content
        public DbSet<Post>Posts { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        //Employees
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Lecturer> Lecturer { get; set; }
        //Images
        public DbSet<Image> Images { get; set; }
        //Students
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentStatus> StudentStatuses { get; set; }
        //Study
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SectionSubject> SectionSubjects { get; set; }
        public DbSet<StudentResult> StudentResults { get; set; }


        //Options
        public AppDbcontext(DbContextOptions<AppDbcontext> options):base(options)
        {

        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbcontext).Assembly);
            ConfigureBaseEntity(modelBuilder);
        }

        private static void ConfigureBaseEntity(ModelBuilder modelBuilder)
        {
            var entityTypes = modelBuilder.Model.GetEntityTypes();

            foreach(var entityType in entityTypes)
            {
                if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                    continue;

                var builder = modelBuilder.Entity(entityType.ClrType);

                builder.Property(nameof(BaseEntity.CreatedAt))
                       .HasDefaultValueSql("GETUTCDATE()");

                builder.Property(nameof(BaseEntity.UpdatedAt))
                       .IsRequired(false);

                builder.HasOne(typeof(User), nameof(BaseEntity.CreatedByUser))
                       .WithMany()
                       .HasForeignKey(nameof(BaseEntity.CreatedByUserId))
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(typeof(User), nameof(BaseEntity.UpdatedByUser))
                       .WithMany()
                       .HasForeignKey(nameof(BaseEntity.UpdatedByUserId))
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }
        //
    }
}
