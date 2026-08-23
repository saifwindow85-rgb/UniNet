using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.SubjectRequests;
using Contracts.Responses.StudyResponses.SubjectResponses;
using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Extensions;
using Domain.Entities.Study;
using Domain.Interfaces.StudyInterfaces.SubjectInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos.StudyRepository
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AppDbcontext _context;
        private readonly Expression<Func<Subject, SubjectDTO>> ToDto = s => new SubjectDTO
        {
            SubjectId = s.SubjectId,
            SubjectName = s.Name,
            Code = s.Code,
            Description = s.Description,
            DepartmentId = s.DepartmentId,
            DepartmentName = s.Department.Name,
        };

        private readonly Expression<Func<Subject, SubjectAuthorizationInfo>> ToInfo = s => new SubjectAuthorizationInfo
        {
            UniversityId = s.Department.College.UniversityId,
            CollegeId = s.Department.CollegeId,
            DepartmentId = s.DepartmentId
        };

        private readonly Expression<Func<Subject, DetaieldSubjectDTO>> ToDetaieldDto = s => new DetaieldSubjectDTO()
        {
            SubjectId = s.SubjectId,
            SubjectName = s.Name,
            Code = s.Code,
            Description = s.Description,
            CreditHours = s.CreditHours,
            DepartmentId = s.DepartmentId,
            DepartmentName = s.Department.Name,
            CreatedAt = s.CreatedAt,
            CreatedByUserId = s.CreatedByUserId,
            CreatedByUserName = s.CreatedByUser.UserName,
            UpdatedAt = s.UpdatedAt,
            UpdatedByUserId = s.UpdatedByUserId,
            UpdatedByUserName = s.UpdatedByUser == null ? null : s.UpdatedByUser.UserName,
        };


        public SubjectRepository(AppDbcontext context)
        {
            _context = context;
        }
        public void Add(Subject subject)
        {
            _context.Subjects.Add(subject);
        }

        public bool Delete(Subject subject)
        {
            if (subject == null)
                return false;
            _context.Subjects.Remove(subject);
            return true;
        }

        public async Task<PagedResult<SubjectDTO>> GetAll(SubjectFilterDTO? filter, int pageNumber, int pageSize)
        {
            if (filter == null)
                filter = new SubjectFilterDTO();

            var query = _context.Subjects.AsNoTracking().OrderBy(s => s.DepartmentId).
                ThenBy(s => s.CreatedAt).ThenBy(s => s.CreditHours).AsQueryable();

            if(filter.DepartmentId.HasValue)
            {
                query = query.Where(s => s.DepartmentId == filter.DepartmentId);
            }

            if(!string.IsNullOrEmpty(filter.Code))
            {
                query = query.Where(s => EF.Functions.Like(s.Code,$"%{filter.Code}%"));
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(s => EF.Functions.Like(s.Name, $"%{filter.Name}%"));
            }

            return await query.Select(ToDto).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<DetaieldSubjectDTO?> GetDetaieldSubjectDTOById(int subjectId)
        {
            return await _context.Subjects.Where(s => s.SubjectId == subjectId).Select(ToDetaieldDto).SingleOrDefaultAsync();
        }

        public async Task<SubjectDTO?> GetDTOById(int subjectId)
        {
            return await _context.Subjects.Where(s => s.SubjectId == subjectId).Select(ToDto).SingleOrDefaultAsync();
        }

        public async Task<Subject?> GetEntityById(int subjectId)
        {
            return await _context.Subjects.FindAsync(subjectId);
        }

        public async Task<SubjectAuthorizationInfo?> GetSubjectAuthorizationInfoAsync(int subjecId)
        {
            return await _context.Subjects.Where(s => s.SubjectId == subjecId).Select(ToInfo).SingleOrDefaultAsync();
        }

        public async Task<PagedResult<SubjectDTO>> GetSubjectsPerDepartment(UserScope? scope, SubjectFilterDTO? filter, int pageNumber, int pageSize)
        {
            if(filter == null)
                filter = new SubjectFilterDTO();

            if(scope == null)
                scope = new UserScope();

           var query = _context.Subjects.AsNoTracking().OrderBy(s => s.DepartmentId).
                ThenBy(s => s.CreatedAt).ThenBy(s => s.CreditHours).AsQueryable();


            if (filter.DepartmentId.HasValue)
            {
                query = query.Where(s => s.DepartmentId == filter.DepartmentId);
            }

            if (!string.IsNullOrEmpty(filter.Code))
            {
                query = query.Where(s => EF.Functions.Like(s.Code, $"%{filter.Code}%"));
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(s => EF.Functions.Like(s.Name, $"%{filter.Name}%"));
            }

            if (!scope.IsGlobal)
            {
                if(scope.UniversityId.HasValue)
                {
                    query = query.Where(s => s.Department.College.UniversityId == scope.UniversityId);
                }

                if(scope.CollegeId.HasValue)
                {
                    query = query.Where(s=>s.Department.CollegeId == scope.CollegeId);
                }

                if(scope.DepartmentId.HasValue)
                {
                    query = query.Where(s => s.DepartmentId == scope.DepartmentId);
                }
            }

            return await query.Select(ToDto).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<bool> IsExistsById(int subjectId)
        {
            return await _context.Subjects.AnyAsync(s => s.SubjectId == subjectId);
        }

        public async Task<bool> IsExistsByName(int departmentId,string name)
        {
            return await _context.Subjects.AnyAsync(s=>s.DepartmentId == departmentId && s.Name == name);
        }
    }
}
