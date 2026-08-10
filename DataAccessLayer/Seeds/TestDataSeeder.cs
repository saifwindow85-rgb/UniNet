using Contracts.Enums;
using DataAccessLayer.Dbcontext;
using Domain.Entities.Academic_Structure;
using Domain.Entities.Content;
using Domain.Entities.Enums;
using Domain.Entities.Employees;
using Domain.Entities.Identity;
using Domain.Entities.Images;
using Domain.Entities.Students;
using Domain.Entities.Study;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Seeds
{
    /// <summary>
    /// Runtime seeder that populates a realistic, interrelated dataset (hundreds of rows)
    /// for development/testing. Runs only when wired in Program.cs under Development.
    /// Idempotent: a sentinel university ("Sana'a University") guards re-runs.
    /// Uses real BCrypt hashes so seeded accounts can actually log in.
    /// </summary>
    public static class TestDataSeeder
    {
        private const string DefaultPassword = "P@ssw0rd123!";

        private static string HashPassword() => BCrypt.Net.BCrypt.HashPassword(DefaultPassword);

        // Name pools (FullName is not unique — only UserName/Email are — so reuse is safe).
        private static readonly string[] FirstNames =
            { "Ahmed", "Mohammed", "Ali", "Hassan", "Khaled", "Omar", "Yusuf", "Layla", "Fatima", "Sara" };
        private static readonly string[] LastNames =
            { "Al-Hadhrami", "Al-Sanani", "Al-Yafai", "Al-Kindi", "Al-Maqtari", "Al-Adeni", "Al-Rimi", "Al-Shami" };

        private static int _nameCounter; // deterministic name cycling
        private static int _studentCounter; // unique student numbering across the run

        public static async Task SeedAsync(AppDbcontext db)
        {
            // ---- Sentinel: skip if our test data already exists ----
            if (await db.Universities.AnyAsync(u => u.Name == "Sana'a University"))
                return;

            var now = DateTime.UtcNow;

            // ---- Reference data (idempotent upserts; also provided by HasData) ----
            var roleIds = await EnsureRolesAsync(db);
            int enrolledStatusId = await EnsureStudentStatusesAsync(db);

            // ---- A loginable Super Admin (creator for audit fields) ----
            int creatorId = await EnsureSuperAdminAsync(db, roleIds["Super Admin"]);

            // ---- Semester (shared) ----
            var semester = new Semester
            {
                Name = "Fall 2025",
                StartDate = new DateTime(2025, 9, 1),
                EndDate = new DateTime(2025, 12, 20),
                IsCurrent = true,
                CreatedAt = now,
                CreatedByUserId = creatorId
            };
            await db.Semesters.AddAsync(semester);
            await db.SaveChangesAsync();

            // ---- Universities: 2 new + the existing "Hadhramout University" from HasData ----
            var sanaa = new University { Name = "Sana'a University", Description = "A national university in the capital.", CreatedAt = now, CreatedByUserId = creatorId };
            var aden = new University { Name = "Aden University", Description = "A coastal university in southern Yemen.", CreatedAt = now, CreatedByUserId = creatorId };
            await db.Universities.AddRangeAsync(sanaa, aden);
            await db.SaveChangesAsync();

            University hadhramout;
            try
            {
                hadhramout = await db.Universities.SingleAsync(u => u.Name == "Hadhramout University");
            }
            catch
            {
                // HasData didn't run (e.g., EnsureCreated path) — create it so the hierarchy is complete.
                hadhramout = new University { Name = "Hadhramout University", Description = "A leading Yemeni university.", CreatedAt = now, CreatedByUserId = creatorId };
                await db.Universities.AddAsync(hadhramout);
                await db.SaveChangesAsync();
            }

            var universities = new List<University> { hadhramout, sanaa, aden };
            var collegeNames = new[] { "Faculty of Engineering", "Faculty of Science", "Faculty of Business" };
            var deptNames = new[] { "Department of Applied Sciences", "Department of Research", "Department of Foundations" };
            var subjectTemplates = new[] { ("Introduction to Fundamentals", "FUND101"), ("Advanced Topics", "ADV201") };

            _studentCounter = 0; // reset for a fresh seed run

            for (int ui = 0; ui < universities.Count; ui++)
            {
                var univ = universities[ui];
                string univKey = univ.Name.Contains("Sana'a") ? "sanaa" : univ.Name.Contains("Aden") ? "aden" : "had";

                // 1 UniversityAdmin per university (scoped to the whole university).
                await CreateAdminAsync(db, PickName(), $"univadmin.{univKey}",
                    univ.UniversityId, null, null, roleIds["UniversityAdmin"], creatorId, now);

                for (int ci = 0; ci < collegeNames.Length; ci++)
                {
                    var college = new College
                    {
                        UniversityId = univ.UniversityId,
                        Name = collegeNames[ci],
                        Description = $"Academic programs at {collegeNames[ci]}.",
                        CreatedAt = now,
                        CreatedByUserId = creatorId
                    };
                    await db.Colleges.AddAsync(college);
                    await db.SaveChangesAsync();

                    // 1 CollegeAdmin per college.
                    await CreateAdminAsync(db, PickName(), $"colladmin.{univKey}.{ci}",
                        univ.UniversityId, college.CollegeId, null, roleIds["CollegeAdmin"], creatorId, now);

                    for (int di = 0; di < deptNames.Length; di++)
                    {
                        var dept = new Department
                        {
                            CollegeId = college.CollegeId,
                            Name = deptNames[di],
                            Description = $"{deptNames[di]} under {collegeNames[ci]}.",
                            CreatedAt = now,
                            CreatedByUserId = creatorId
                        };
                        await db.Departments.AddAsync(dept);
                        await db.SaveChangesAsync();

                        // 1 DepartmentAdmin per department.
                        await CreateAdminAsync(db, PickName(), $"deptadmin.{univKey}.{ci}{di}",
                            univ.UniversityId, college.CollegeId, dept.DepartmentId, roleIds["DepartmentAdmin"], creatorId, now);

                        // Subjects for this department (unique per DepartmentId).
                        var subjects = subjectTemplates.Select(t => new Subject
                        {
                            Code = t.Item2,
                            Name = t.Item1,
                            Description = $"{t.Item1} — {deptNames[di]}.",
                            CreditHours = 3,
                            DepartmentId = dept.DepartmentId,
                            CreatedAt = now,
                            CreatedByUserId = creatorId
                        }).ToList();
                        await db.Subjects.AddRangeAsync(subjects);
                        await db.SaveChangesAsync();

                        // 1 Batch per department.
                        var batch = new Batch
                        {
                            DepartmentId = dept.DepartmentId,
                            Name = "Batch 2025",
                            BatchYear = 2025,
                            Description = "First cohort under the current curriculum.",
                            CreatedAt = now,
                            CreatedByUserId = creatorId
                        };
                        await db.Batches.AddAsync(batch);
                        await db.SaveChangesAsync();

                        // 2 Sections per batch.
                        for (int si = 0; si < 2; si++)
                        {
                            var section = new Section
                            {
                                BatchId = batch.BatchId,
                                Name = $"Section {(char)('A' + si)}",
                                CreatedAt = now,
                                CreatedByUserId = creatorId
                            };
                            await db.Sections.AddAsync(section);
                            await db.SaveChangesAsync();

                            // Link this section to each of the department's subjects in the current semester.
                            foreach (var subj in subjects)
                            {
                                await db.SectionSubjects.AddAsync(new SectionSubject
                                {
                                    SectionId = section.SectionId,
                                    SubjectId = subj.SubjectId,
                                    SemesterId = semester.SemesterId,
                                    LecturerName = "Staff Lecturer",
                                    CreatedAt = now,
                                    CreatedByUserId = creatorId
                                });
                            }
                            await db.SaveChangesAsync();

                            // 4 Students per section.
                            await CreateStudentsAsync(db, univ.UniversityId, batch.BatchId, section.SectionId,
                                enrolledStatusId, roleIds["Student"], creatorId, now, count: 4);
                        }
                    }
                }
            }

            // ---- A few Lecturers (Users with role "Lecturer", no Employee record) ----
            for (int li = 0; li < 5; li++)
            {
                var lecturer = new User
                {
                    FullName = PickName(),
                    UserName = $"lecturer{li}",
                    PasswordHash = HashPassword(),
                    Email = $"lecturer{li}@seed.test",
                    IsActive = true,
                    Type = UserType.Employee,
                    UniversityId = hadhramout.UniversityId,
                    CreatedAt = now,
                    CreatedByUserId = creatorId
                };
                await db.Users.AddAsync(lecturer);
                await db.SaveChangesAsync();
                await db.UserRoles.AddAsync(new UserRole { UserId = lecturer.UserId, RoleId = roleIds["Lecturer"] });
                await db.SaveChangesAsync();
            }

            // ---- Content & Images (a couple of each) ----
            var img1 = new Image { FileName = "welcome.jpg", FilePath = "/uploads/welcome.jpg", FileSize = 102400, UploadedAt = now, UploadedByUserId = creatorId, UpdatedAt = now };
            var img2 = new Image { FileName = "library.jpg", FilePath = "/uploads/library.jpg", FileSize = 204800, UploadedAt = now, UploadedByUserId = creatorId, UpdatedAt = now };
            await db.Images.AddRangeAsync(img1, img2);
            await db.SaveChangesAsync();

            await db.Posts.AddRangeAsync(
                new Post { Title = "Welcome to Fall 2025", Content = "We welcome all students to the new academic year.", Type = EnContentType.University, ImageId = img1.ImageId, CreatedAt = now, CreatedByUserId = creatorId },
                new Post { Title = "Library Hours", Content = "The library is open 8 AM – 8 PM daily.", Type = EnContentType.College, ImageId = img2.ImageId, CreatedAt = now, CreatedByUserId = creatorId });

            await db.Announcements.AddRangeAsync(
                new Announcement { Title = "Midterm Schedule", Content = "Midterm exams begin December 1st.", Type = EnContentType.University, CreatedAt = now, CreatedByUserId = creatorId },
                new Announcement { Title = "Holiday Notice", Content = "The university will be closed for the national holiday.", Type = EnContentType.College, CreatedAt = now, CreatedByUserId = creatorId });

            await db.SaveChangesAsync();
        }

        // -------- Helpers --------

        private static async Task<Dictionary<string, int>> EnsureRolesAsync(AppDbcontext db)
        {
            var names = new[] { "Super Admin", "UniversityAdmin", "CollegeAdmin", "DepartmentAdmin", "Lecturer", "Student","BatchAdmin" };
            foreach (var name in names)
            {
                if (!await db.Roles.AnyAsync(r => r.Name == name))
                    await db.Roles.AddAsync(new Role { Name = name });
            }
            await db.SaveChangesAsync();
            return await db.Roles.ToDictionaryAsync(r => r.Name, r => r.RoleId);
        }

        private static async Task<int> EnsureStudentStatusesAsync(AppDbcontext db)
        {
            var defs = new[] { ("Enrolled", "Actively enrolled and progressing."), ("Graduated", "Completed all requirements."), ("Suspended", "Temporarily suspended.") };
            foreach (var (name, desc) in defs)
            {
                if (!await db.StudentStatuses.AnyAsync(s => s.Name == name))
                    await db.StudentStatuses.AddAsync(new StudentStatus { Name = name, Description = desc });
            }
            await db.SaveChangesAsync();
            return await db.StudentStatuses.Where(s => s.Name == "Enrolled").Select(s => s.StatusId).FirstAsync();
        }

        private static async Task<int> EnsureSuperAdminAsync(AppDbcontext db, int superRoleId)
        {
            var admin = await db.Users.FirstOrDefaultAsync(u => u.UserName == "Ahmed.HU");
            if (admin is null)
            {
                admin = new User
                {
                    FullName = "Dr. Ahmed Al-Hadrami",
                    UserName = "Ahmed.HU",
                    PasswordHash = HashPassword(),
                    Email = "ahmed@hu.edu.ye",
                    PhoneNumber = "+967777000111",
                    IsActive = true,
                    Type = UserType.SystemAdmin,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = null
                };
                await db.Users.AddAsync(admin);
                await db.SaveChangesAsync();
                await db.UserRoles.AddAsync(new UserRole { UserId = admin.UserId, RoleId = superRoleId });
                await db.SaveChangesAsync();
            }
            else
            {
                // The HasData seed used a fake hash — overwrite it with a real one so login works.
                admin.PasswordHash = HashPassword();
                await db.SaveChangesAsync();
            }
            return admin.UserId;
        }

        private static async Task CreateAdminAsync(AppDbcontext db, string fullName, string userName,
            int universityId, int? collegeId, int? departmentId, int roleId, int creatorId, DateTime now)
        {
            var user = new User
            {
                FullName = fullName,
                UserName = userName,
                PasswordHash = HashPassword(),
                Email = $"{userName}@seed.test",
                IsActive = true,
                Type = UserType.Employee,
                UniversityId = universityId,
                CreatedAt = now,
                CreatedByUserId = creatorId
            };
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            await db.Employees.AddAsync(new Employee
            {
                UserId = user.UserId,
                UniversityId = universityId,
                CollegeId = collegeId,
                DepartmentId = departmentId
            });
            await db.UserRoles.AddAsync(new UserRole { UserId = user.UserId, RoleId = roleId });
            await db.SaveChangesAsync();
        }

        private static async Task CreateStudentsAsync(AppDbcontext db, int universityId, int batchId, int sectionId,
            int statusId, int studentRoleId, int creatorId, DateTime now, int count)
        {
            var users = new List<User>(count);
            for (int i = 0; i < count; i++)
            {
                _studentCounter++;
                string userName = $"student{_studentCounter}";
                users.Add(new User
                {
                    FullName = PickName(),
                    UserName = userName,
                    PasswordHash = HashPassword(),
                    Email = $"{userName}@seed.test",
                    IsActive = true,
                    Type = UserType.Student,
                    UniversityId = universityId,
                    CreatedAt = now,
                    CreatedByUserId = creatorId
                });
            }
            await db.Users.AddRangeAsync(users);
            await db.SaveChangesAsync(); // generate UserIds

            // '_studentCounter' now equals (start + count); the first student in this batch is (start - count + 1).
            int baseNumber = _studentCounter - count + 1;
            for (int i = 0; i < users.Count; i++)
            {
                await db.UserRoles.AddAsync(new UserRole { UserId = users[i].UserId, RoleId = studentRoleId });
                await db.Students.AddAsync(new Student
                {
                    UserId = users[i].UserId,
                    StudentNumber = $"STU{baseNumber + i:00000}", // unique, stable numbering
                    BatchId = batchId,
                    SectionId = sectionId,
                    StatusId = statusId,
                    EnrollmentDate = now
                });
            }
            await db.SaveChangesAsync();
        }

        private static string PickName()
        {
            string first = FirstNames[_nameCounter % FirstNames.Length];
            string last = LastNames[(_nameCounter / FirstNames.Length) % LastNames.Length];
            _nameCounter++;
            return $"{first} {last}";
        }
    }
}