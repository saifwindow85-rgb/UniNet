using BCrypt.Net;
using Domain.Entities.Academic_Structure;
using Domain.Entities.Content;
using Domain.Entities.Employees;
using Contracts.Enums;
using Domain.Entities.Identity;
using Domain.Entities.Images;
using Domain.Entities.Students;
using Domain.Entities.Study;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Seeds
{
    public static class SeedData
    {
        // ----------------------------------------------------------
        // 1. الهوية والمستخدمون (Identity & Users)
        // ----------------------------------------------------------

        public static List<Role> GetRoles() => new()
        {
            new Role { RoleId = 1, Name = "Super Admin" },
            new Role { RoleId = 2, Name = "UniversityAdmin" },
            new Role { RoleId = 3, Name = "CollegeAdmin" },
            new Role { RoleId = 4, Name = "DepartmentAdmin" },
            new Role { RoleId = 5, Name = "Lecturer" },
            new Role { RoleId = 6, Name = "Student" },
            new Role { RoleId =7,Name = "BatchAdmin"}
        };

        public static List<User> GetUsers() => new()
        {
            new User
            {
                UserId = 1,
                FullName = "Dr. Ahmed Al-Hadrami",
                UserName = "Ahmed.HU",
                // استخدم مكتبة BCrypt لتوليد هذا النص من كلمة المرور "P@ssw0rd123!"
                PasswordHash =  "$2a$11$co8IwpHzsf8C.PS/a/ZYUeQA//rUxp2epNB70c5qkNK0YUkS/RO46",
                Email = "ahmed@hu.edu.ye",
                PhoneNumber = "+967 777000111",
                IsActive = true,
                CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedByUserId = 1 // (المستخدم الأول أنشأ نفسه في النظام)
            }
        };

        public static List<UserRole> GetUserRoles() => new()
        {
            new UserRole { UserId = 1, RoleId = 1 } // المستخدم الأول هو Super Admin
        };


        // ----------------------------------------------------------
        // 2. الهيكل الأكاديمي (Academic Structure)
        // ----------------------------------------------------------

        public static List<University> GetUniversities() => new()
        {
            new University
            {
                UniversityId = 1,
                Name = "Hadhramout University",
                Description = "A leading Yemeni university committed to academic excellence and community development.",
                CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedByUserId = 1
            }
        };

        public static List<College> GetColleges() => new()
        {
            new College
            {
                CollegeId = 1,
                UniversityId = 1,
                Name = "Faculty of Computer Science and Information Technology",
                Description = "Specializes in Software Engineering, AI, Cybersecurity, and Data Science.",
                CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedByUserId = 1
            }
        };

        public static List<Department> GetDepartments() => new()
        {
            new Department
            {
                DepartmentId = 1,
                CollegeId = 1,
                Name = "Information Technology Department",
                Description = "Focuses on networking, database management, web development, and system security.",
                CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedByUserId = 1
            }
        };

        public static List<Batch> GetBatches() => new()
        {
            new Batch
            {
                BatchId = 1,
                DepartmentId = 1,
                Name = "Batch 2025",
                BatchYear = 2025,
                Description = "First cohort of the new curriculum.",
                CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedByUserId = 1
            }
        };

        public static List<Section> GetSections() => new()
        {
            new Section
            {
                SectionId = 1,
                BatchId = 1,
                Name = "Section A",
                CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedByUserId = 1
            }
        };


        // ----------------------------------------------------------
        // 3. الطلاب (Students)
        // ----------------------------------------------------------

        public static List<StudentStatus> GetStudentStatuses() => new()
        {
            new StudentStatus
            {
                StatusId = 1,
                Name = "Enrolled",
                Description = "Actively enrolled and progressing in studies."
            },
            new StudentStatus
            {
                StatusId = 2,
                Name = "Graduated",
                Description = "Completed all academic requirements."
            }
        };

        public static List<Student> GetStudents() => new()
        {
            new Student
            {
                StudentId = 1,
                UserId = 1,          // نفس المستخدم (يمكن جعله طالباً أيضاً، أو سننشئ مستخدم آخر. هنا سنفترض أن Admin هو طالب تجريبي)
                StudentNumber = "20251001",
                BatchId = 1,
                SectionId = 1,
                StatusId = 1,
                EnrollmentDate = new DateTime(2025, 9, 15, 0, 0, 0, DateTimeKind.Utc)
            }
        };


        // ----------------------------------------------------------
        // 4. الموظفون والمحاضرون (Employees & Lecturers)
        // ----------------------------------------------------------

        public static List<Employee> GetEmployees() => new()
        {
            new Employee
            {
                EmployeeId = 1,
                UserId = 1,
                UniversityId = 1,
                CollegeId = 1,
                DepartmentId = 1
            }
        };



        // ----------------------------------------------------------
        // 5. الدراسات (Study: Subjects, Semesters, SectionSubjects)
        // ----------------------------------------------------------

        public static List<Subject> GetSubjects() => new()
        {
            new Subject
            {
                SubjectId = 1,
                Code = "CS101",
                Name = "Introduction to Programming",
                Description = "Fundamentals of programming using C# and .NET.",
                CreditHours = 3,
                DepartmentId = 1,
                CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedByUserId = 1
            },
            new Subject
            {
                SubjectId = 2,
                Code = "CS205",
                Name = "Database Systems",
                Description = "Relational database design, SQL, and Entity Framework Core.",
                CreditHours = 3,
                DepartmentId = 1,
                CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedByUserId = 1
            }
        };

        public static List<Semester> GetSemesters() => new()
        {
            new Semester
            {
                SemesterId = 1,
                Name = "Fall 2025",
                StartDate = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2025, 12, 20, 0, 0, 0, DateTimeKind.Utc),
                UniversityId = 1,
                IsCurrent = true,
                CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedByUserId = 1
            }
        };

        public static List<SectionSubject> GetSectionSubjects() => new()
        {
            new SectionSubject
            {
                SectionSubjectId = 1,
                SectionId = 1,
                SubjectId = 1,
                SemesterId = 1,
                LecturerName = "Ahmed",
                CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedByUserId = 1
            }
        };


        // ----------------------------------------------------------
        // 6. المحتوى والصور (Content & Images)
        // ----------------------------------------------------------



        // ----------------------------------------------------------
        // 7. النتائج الأكاديمية (Student Results)
        // ----------------------------------------------------------

        public static List<StudentResult> GetStudentResults()
        {
            // الدرجات صارت private set؛ تُكتب فقط عبر SetGrades الذي يفرض حدود GradeConstants.
            // Total عمود محسوب في قاعدة البيانات، لا يُعيَّن هنا.
            var result = new StudentResult
            {
                StudentResultId = 1,
                StudentId = 1,
                SectionSubjectId = 1,
                CreatedByUserId = 1,
                CreatedAt = new DateTime(2025, 12, 25, 0, 0, 0, DateTimeKind.Utc)
            };
            result.SetGrades(27, 16, 45); // ضمن الحدود 30 / 20 / 50 (المجموع من 100)

            return new List<StudentResult> { result };
        }
    }
}
