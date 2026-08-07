using Contracts.Common.AuthorizationInfos.StudentAuthorizationInfo;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudentRequests;
using Contracts.Responses.StudentResponses;
using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Extensions;
using Domain.Entities.Students;
using Domain.Interfaces.StudentInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos.StudentRepository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbcontext _context;
        private readonly Expression<Func<Student, StudentDTO>> ToDTO = s => new StudentDTO
        {
            StudentId = s.StudentId,
            FullName = s.User.FullName,
            UserId = s.UserId,
            UserName = s.User.UserName,
            StudentNumber = s.StudentNumber,
            BatchId = s.BatchId,
            BatchName = s.Batch.Name,
            SectionId = s.SectionId,
            SectionName = s.Section == null ? null : s.Section.Name,
            StatusName = s.Status.Name,

        };

        private readonly Expression<Func<Student, StudentAuthorizationInfo>> ToInfo = s => new StudentAuthorizationInfo
        {
            StudentId = s.StudentId,
            UniversityId = s.Batch.Department.College.UniversityId,
            CollegeId = s.Batch.Department.CollegeId,
            DepartmentId = s.Batch.DepartmentId,
            BatchId = s.BatchId,
        };
        public StudentRepository(AppDbcontext context)
        {
            _context = context;
        }
        public void Add(Student student)
        {
         _context.Students.Add(student);
        }

        public async Task<bool> ExistsByStudentNumber(string studentNumber)
        {
            return await _context.Students.AnyAsync(s=>s.StudentNumber == studentNumber);
        }

        public async Task<StudentDTO?> GetDTOById(int studentId)
        {
            return await _context.Students
                .Where(s => s.StudentId == studentId)
                .Select(ToDTO)
                .SingleOrDefaultAsync();
        }

        public Task<StudentDTO?> GetDTOByStudentNumber(string studentNumber)
        {
            return _context.Students
                .Where(s => s.StudentNumber == studentNumber)
                .Select(ToDTO)
                .SingleOrDefaultAsync();
        }

        public async Task<Student?> GetEntityById(int studentId)
        {
            return await _context.Students.FindAsync(studentId);
        }

        public async Task<StudentAuthorizationInfo?> GetStudentAuthorizationInfoAsync(int studentId)
        {
            return await _context.Students.Where(s => s.StudentId == studentId).Select(ToInfo).SingleOrDefaultAsync();
        }

        public Task<StudentAuthorizationInfo?> GetStudentAuthorizationInfoAsyncByUserId(int userId)
        {
            return _context.Students.Where(s => s.UserId == userId).Select(ToInfo).SingleOrDefaultAsync();
        }

        public Task<PagedResult<StudentDTO>> GetStudents(UserScope? scope, StudentFilter? filter, int pageNumber, int pageSize)
        {
            var query = _context.Students.AsNoTracking().AsQueryable();
            if (filter == null)
                filter = new StudentFilter();

            if (scope == null || (scope.UniversityId == null && scope.CollegeId == null && scope.DepartmentId == null && scope.BatchId == null))
                scope = new UserScope();

            if (!scope.BatchId.HasValue)
            {
                if (scope.UniversityId.HasValue)
                {
                    query = query.Where(s => s.Batch.Department.College.UniversityId == scope.UniversityId);
                }
                if (scope.CollegeId.HasValue)
                {
                    query = query.Where(s => s.Batch.Department.CollegeId == scope.CollegeId);
                }

                if (scope.DepartmentId.HasValue)
                {
                    query = query.Where(s => s.Batch.DepartmentId == scope.DepartmentId);
                }

                if (filter.BatchId.HasValue)
                {
                    query = query.Where(s => s.BatchId == filter.BatchId);
                }

            }
            else
            {
                query = query.Where(s => s.BatchId == scope.BatchId);
            }
            if (!string.IsNullOrEmpty(filter.FullName))
            {
                query = query.Where(s => EF.Functions.Like(s.User.FullName, $"%{filter.FullName}%"));
            }

            if (!string.IsNullOrEmpty(filter.StudentNumber))
            {
                query = query.Where(s => EF.Functions.Like(s.StudentNumber, $"%{filter.StudentNumber}%"));
            }
             
            if(filter.Status.HasValue)
            {
                query = query.Where(s => s.StatusId == filter.Status);
            }

            if(filter.IsActive.HasValue)
            {
                query = query.Where(s => s.User.IsActive == filter.IsActive);
            }

            return query.Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }
    }
}
