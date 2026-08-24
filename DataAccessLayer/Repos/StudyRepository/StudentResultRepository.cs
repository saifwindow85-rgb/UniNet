using Contracts.Common.AuthorizationInfos.StudentAuthorizationInfo;
using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.StudentResultRequests;
using Contracts.Responses.StudyResponses.StudentResultResponses;
using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Extensions;
using Domain.Entities.Study;
using Domain.Interfaces.StudyInterfaces.StudentResultInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos.StudyRepository
{
    public class StudentResultRepository : IStudentResultRepository
    {
        private readonly AppDbcontext _context;

        private readonly Expression<Func<StudentResult, DetaieldStudentResultDTO>> ToDetaieldDto = sr => new DetaieldStudentResultDTO
        {
            StudentResultId = sr.StudentResultId,
            StudentId = sr.StudentId,
            StudentName = sr.Student.User.FullName,
            StudentNumber = sr.Student.StudentNumber,
            SectionSubjectId = sr.SectionSubjectId,
            SubjectId = sr.SectionSubject.SubjectId,
            SubjectName = sr.SectionSubject.Subject.Name,
            SubjectCode = sr.SectionSubject.Subject.Code,
            LecturerName = sr.SectionSubject.LecturerName,
            SectionId = sr.SectionSubject.SectionId,
            SectionName = sr.SectionSubject.Section.Name,
            SemesterId = sr.SectionSubject.SemesterId,
            SemesterName = sr.SectionSubject.Semester.Name,
            Midterm = sr.Midterm,
            Practical = sr.Practical,
            Final = sr.Final,
            Total = sr.Total,
            CreatedAt = sr.CreatedAt,
            CreatedByUserId = sr.CreatedByUserId,
            CreatedByUserName = sr.CreatedByUser.UserName,
            UpdatedAt = sr.UpdatedAt,
            UpdatedByUserId = sr.UpdatedByUserId,
            UpdatedByUserName = sr.UpdatedByUser == null ? null : sr.UpdatedByUser.UserName,
        };

        private readonly Expression<Func<StudentResult, StudentResultAuthorizationInfo>> ToInfo = sr => new StudentResultAuthorizationInfo
        {
            UniversityId = sr.SectionSubject.Section.Batch.Department.College.UniversityId,
            CollegeId = sr.SectionSubject.Section.Batch.Department.CollegeId,
            DepartmentId = sr.SectionSubject.Section.Batch.DepartmentId,
            BatchId = sr.SectionSubject.Section.BatchId,
            SectionId = sr.SectionSubject.SectionId,
        };

        public StudentResultRepository(AppDbcontext context)
        {
            _context = context;
        }

        public void Add(StudentResult studentResult)
        {
            _context.StudentResults.Add(studentResult);
        }

        public bool Delete(StudentResult studentResult)
        {
            if (studentResult == null)
                return false;
            _context.StudentResults.Remove(studentResult);
            return true;
        }

        public async Task<StudentResult?> GetEntityById(int studentResultId)
        {
            return await _context.StudentResults.FindAsync(studentResultId);
        }

        public async Task<DetaieldStudentResultDTO?> GetDetaieldStudentResultDTOById(int studentResultId)
        {
            return await _context.StudentResults.Where(sr => sr.StudentResultId == studentResultId).Select(ToDetaieldDto).SingleOrDefaultAsync();
        }

        public async Task<PagedResult<DetaieldStudentResultDTO>> GetAll(StudentResultFilterDTO? filter, int pageNumber, int pageSize)
        {
            if (filter == null)
                filter = new StudentResultFilterDTO();

            var query = _context.StudentResults.AsNoTracking()
                .OrderBy(sr => sr.SectionSubject.SemesterId).ThenBy(sr => sr.StudentId).AsQueryable();

            query = ApplyFilter(query, filter);

            return await query.Select(ToDetaieldDto).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<StudentResultAuthorizationInfo?> GetStudentResultAuthorizationInfoAsync(int studentResultId)
        {
            return await _context.StudentResults.Where(sr => sr.StudentResultId == studentResultId).Select(ToInfo).SingleOrDefaultAsync();
        }

        public async Task<StudentAuthorizationInfo?> GetStudentAuthorizationInfoAsync(int studentId)
        {
            return await _context.Students.Where(s => s.StudentId == studentId).Select(s => new StudentAuthorizationInfo
            {
                StudentId = s.StudentId,
                UniversityId = s.Batch.Department.College.UniversityId,
                CollegeId = s.Batch.Department.CollegeId,
                DepartmentId = s.Batch.DepartmentId,
                BatchId = s.BatchId,
            }).SingleOrDefaultAsync();
        }

        public async Task<bool> IsExistsById(int studentResultId)
        {
            return await _context.StudentResults.AnyAsync(sr => sr.StudentResultId == studentResultId);
        }

        public async Task<bool> IsAlreadyRecorded(int studentId, int sectionSubjectId)
        {
            return await _context.StudentResults.AnyAsync(sr => sr.StudentId == studentId && sr.SectionSubjectId == sectionSubjectId);
        }

        public async Task<List<StudentSemesterResultDTO>> GetStudentResults(int studentId, StudentResultFilterDTO? filter)
        {
            if (filter == null)
                filter = new StudentResultFilterDTO();

            var query = _context.StudentResults.AsNoTracking().Where(sr => sr.StudentId == studentId);
            query = ApplyFilter(query, filter);

            // كشف طالب واحد: الترتيب حسب SemesterId.
            var rows = await query.OrderBy(sr => sr.SectionSubject.SemesterId).Select(ToRow).ToListAsync();

            return GroupIntoSemesterResults(rows);
        }

        public async Task<List<StudentSemesterResultDTO>> GetAllStudentsResults(UserScope? scope, StudentResultFilterDTO? filter)
        {
            if (filter == null)
                filter = new StudentResultFilterDTO();

            if (scope == null)
                scope = new UserScope();

            var query = _context.StudentResults.AsNoTracking().AsQueryable();
            query = ApplyFilter(query, filter);

            if (!scope.IsGlobal)
            {
                if (scope.UniversityId.HasValue)
                    query = query.Where(sr => sr.SectionSubject.Section.Batch.Department.College.UniversityId == scope.UniversityId);

                if (scope.CollegeId.HasValue)
                    query = query.Where(sr => sr.SectionSubject.Section.Batch.Department.CollegeId == scope.CollegeId);

                if (scope.DepartmentId.HasValue)
                    query = query.Where(sr => sr.SectionSubject.Section.Batch.DepartmentId == scope.DepartmentId);

                if (scope.BatchId.HasValue)
                    query = query.Where(sr => sr.SectionSubject.Section.BatchId == scope.BatchId);
            }

            // تقرير كل الطلاب: الترتيب حسب Section => Batch => Department => College => University، ثم الطالب ثم الفصل.
            var rows = await query
                .OrderBy(sr => sr.SectionSubject.SectionId)
                .ThenBy(sr => sr.SectionSubject.Section.BatchId)
                .ThenBy(sr => sr.SectionSubject.Section.Batch.DepartmentId)
                .ThenBy(sr => sr.SectionSubject.Section.Batch.Department.CollegeId)
                .ThenBy(sr => sr.SectionSubject.Section.Batch.Department.College.UniversityId)
                .ThenBy(sr => sr.StudentId)
                .ThenBy(sr => sr.SectionSubject.SemesterId)
                .Select(ToRow).ToListAsync();

            return GroupIntoSemesterResults(rows);
        }

        // صفّ مسطّح يحمل مفاتيح التجميع والترتيب مع بيانات الدرجة.
        private static readonly Expression<Func<StudentResult, ResultRow>> ToRow = sr => new ResultRow
        {
            StudentResultId = sr.StudentResultId,
            StudentId = sr.StudentId,
            StudentName = sr.Student.User.FullName,
            StudentNumber = sr.Student.StudentNumber,
            SectionId = sr.SectionSubject.SectionId,
            SectionName = sr.SectionSubject.Section.Name,
            BatchId = sr.SectionSubject.Section.BatchId,
            BatchName = sr.SectionSubject.Section.Batch.Name,
            DepartmentId = sr.SectionSubject.Section.Batch.DepartmentId,
            DepartmentName = sr.SectionSubject.Section.Batch.Department.Name,
            SemesterId = sr.SectionSubject.SemesterId,
            SemesterName = sr.SectionSubject.Semester.Name,
            SectionSubjectId = sr.SectionSubjectId,
            SubjectId = sr.SectionSubject.SubjectId,
            SubjectName = sr.SectionSubject.Subject.Name,
            SubjectCode = sr.SectionSubject.Subject.Code,
            LecturerName = sr.SectionSubject.LecturerName,
            Midterm = sr.Midterm,
            Practical = sr.Practical,
            Final = sr.Final,
            Total = sr.Total,
        };

        // التجميع حسب (الطالب، الفصل) مع الحفاظ على ترتيب الاستعلام، وحساب المعدّل = مجموع Totals ÷ العدد.
        private static List<StudentSemesterResultDTO> GroupIntoSemesterResults(List<ResultRow> rows)
        {
            return rows
                .GroupBy(r => new { r.StudentId, r.SemesterId })
                .Select(g =>
                {
                    var first = g.First();
                    return new StudentSemesterResultDTO
                    {
                        StudentId = first.StudentId,
                        StudentName = first.StudentName,
                        StudentNumber = first.StudentNumber,
                        SectionId = first.SectionId,
                        SectionName = first.SectionName,
                        BatchId = first.BatchId,
                        BatchName = first.BatchName,
                        DepartmentId = first.DepartmentId,
                        DepartmentName = first.DepartmentName,
                        SemesterId = first.SemesterId,
                        SemesterName = first.SemesterName,
                        Results = g.Select(r => new StudentResultItemDTO
                        {
                            StudentResultId = r.StudentResultId,
                            SectionSubjectId = r.SectionSubjectId,
                            SubjectId = r.SubjectId,
                            SubjectName = r.SubjectName,
                            SubjectCode = r.SubjectCode,
                            LecturerName = r.LecturerName,
                            Midterm = r.Midterm,
                            Practical = r.Practical,
                            Final = r.Final,
                            Total = r.Total,
                        }).ToList(),
                        Grade = Math.Round(g.Sum(r => r.Total) / g.Count(), 2),
                    };
                })
                .ToList();
        }

        private static IQueryable<StudentResult> ApplyFilter(IQueryable<StudentResult> query, StudentResultFilterDTO filter)
        {
            if (filter.StudentId.HasValue)
                query = query.Where(sr => sr.StudentId == filter.StudentId);

            if (filter.SectionSubjectId.HasValue)
                query = query.Where(sr => sr.SectionSubjectId == filter.SectionSubjectId);

            if (filter.SubjectId.HasValue)
                query = query.Where(sr => sr.SectionSubject.SubjectId == filter.SubjectId);

            if (filter.SemesterId.HasValue)
                query = query.Where(sr => sr.SectionSubject.SemesterId == filter.SemesterId);

            if (filter.SectionId.HasValue)
                query = query.Where(sr => sr.SectionSubject.SectionId == filter.SectionId);

            if (!string.IsNullOrEmpty(filter.StudentName))
                query = query.Where(sr => EF.Functions.Like(sr.Student.User.FullName, $"%{filter.StudentName}%"));

            if (!string.IsNullOrEmpty(filter.StudentNumber))
                query = query.Where(sr => EF.Functions.Like(sr.Student.StudentNumber, $"%{filter.StudentNumber}%"));

            if (!string.IsNullOrEmpty(filter.SubjectName))
                query = query.Where(sr => EF.Functions.Like(sr.SectionSubject.Subject.Name, $"%{filter.SubjectName}%"));

            if (!string.IsNullOrEmpty(filter.SubjectCode))
                query = query.Where(sr => EF.Functions.Like(sr.SectionSubject.Subject.Code, $"%{filter.SubjectCode}%"));

            if (!string.IsNullOrEmpty(filter.LecturerName))
                query = query.Where(sr => EF.Functions.Like(sr.SectionSubject.LecturerName, $"%{filter.LecturerName}%"));

            return query;
        }

        private class ResultRow
        {
            public int StudentResultId { get; set; }
            public int StudentId { get; set; }
            public string StudentName { get; set; } = null!;
            public string StudentNumber { get; set; } = null!;
            public int SectionId { get; set; }
            public string SectionName { get; set; } = null!;
            public int BatchId { get; set; }
            public string BatchName { get; set; } = null!;
            public int DepartmentId { get; set; }
            public string DepartmentName { get; set; } = null!;
            public int SemesterId { get; set; }
            public string SemesterName { get; set; } = null!;
            public int SectionSubjectId { get; set; }
            public int SubjectId { get; set; }
            public string SubjectName { get; set; } = null!;
            public string SubjectCode { get; set; } = null!;
            public string LecturerName { get; set; } = null!;
            public decimal Midterm { get; set; }
            public decimal Practical { get; set; }
            public decimal Final { get; set; }
            public decimal Total { get; set; }
        }
    }
}
