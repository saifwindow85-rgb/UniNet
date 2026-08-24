using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.SemesterRequests;
using Contracts.Responses.StudyResponses.SemesterResponses;
using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Extensions;
using Domain.Entities.Study;
using Domain.Interfaces.StudyInterfaces.SemesterInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos.StudyRepository
{
    public class SemesterRepository : ISemesterRepository
    {
        private readonly AppDbcontext _context;

        private readonly Expression<Func<Semester, SemesterDTO>> ToDto = s => new SemesterDTO
        {
            SemesterId = s.SemesterId,
            Name = s.Name,
            UniversityId = s.UniversityId,
            UniversityName = s.University.Name,
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            IsCurrent = s.IsCurrent,
        };

        private readonly Expression<Func<Semester, SemesterAuthorizationInfo>> ToInfo = s => new SemesterAuthorizationInfo
        {
            UniversityId = s.UniversityId,
        };

        private readonly Expression<Func<Semester, DetaieldSemesterDTO>> ToDetaieldDto = s => new DetaieldSemesterDTO
        {
            SemesterId = s.SemesterId,
            Name = s.Name,
            UniversityId = s.UniversityId,
            UniversityName = s.University.Name,
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            IsCurrent = s.IsCurrent,
            CreatedAt = s.CreatedAt,
            CreatedByUserId = s.CreatedByUserId,
            CreatedByUserName = s.CreatedByUser.UserName,
            UpdatedAt = s.UpdatedAt,
            UpdatedByUserId = s.UpdatedByUserId,
            UpdatedByUserName = s.UpdatedByUser == null ? null : s.UpdatedByUser.UserName,
        };

        public SemesterRepository(AppDbcontext context)
        {
            _context = context;
        }

        public void Add(Semester semester)
        {
            _context.Semesters.Add(semester);
        }

        public bool Delete(Semester semester)
        {
            if (semester == null)
                return false;
            _context.Semesters.Remove(semester);
            return true;
        }

        public async Task<PagedResult<SemesterDTO>> GetAll(SemesterFilterDTO? filter, int pageNumber, int pageSize)
        {
            if (filter == null)
                filter = new SemesterFilterDTO();

            var query = _context.Semesters.AsNoTracking().OrderBy(s => s.UniversityId).ThenByDescending(s => s.StartDate).AsQueryable();

            if (filter.UniversityId.HasValue)
            {
                query = query.Where(s => s.UniversityId == filter.UniversityId);
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(s => EF.Functions.Like(s.Name, $"%{filter.Name}%"));
            }

            if (filter.IsCurrent.HasValue)
            {
                query = query.Where(s => s.IsCurrent == filter.IsCurrent);
            }

            return await query.Select(ToDto).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<PagedResult<SemesterDTO>> GetSemestersPerUniversity(UserScope? scope, SemesterFilterDTO? filter, int pageNumber, int pageSize)
        {
            if (filter == null)
                filter = new SemesterFilterDTO();

            if (scope == null)
                scope = new UserScope();

            var query = _context.Semesters.AsNoTracking().OrderBy(s => s.UniversityId).ThenByDescending(s => s.StartDate).AsQueryable();

            if(filter.UniversityId.HasValue && scope.IsGlobal)
            {
                query = query.Where(s => s.UniversityId == filter.UniversityId);
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(s => EF.Functions.Like(s.Name, $"%{filter.Name}%"));
            }

            if (filter.IsCurrent.HasValue)
            {
                query = query.Where(s => s.IsCurrent == filter.IsCurrent);
            }

            if (!scope.IsGlobal)
            {
                if (scope.UniversityId.HasValue)
                {
                    query = query.Where(s => s.UniversityId == scope.UniversityId);
                }
            }

            return await query.Select(ToDto).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<SemesterDTO?> GetDTOById(int semesterId)
        {
            return await _context.Semesters.Where(s => s.SemesterId == semesterId).Select(ToDto).SingleOrDefaultAsync();
        }

        public async Task<DetaieldSemesterDTO?> GetDetaieldSemesterDTOById(int semesterId)
        {
            return await _context.Semesters.Where(s => s.SemesterId == semesterId).Select(ToDetaieldDto).SingleOrDefaultAsync();
        }

        public async Task<Semester?> GetEntityById(int semesterId)
        {
            return await _context.Semesters.FindAsync(semesterId);
        }

        public async Task<Semester?> GetCurrentSemesterEntity(int universityId)
        {
            return await _context.Semesters.SingleOrDefaultAsync(s => s.UniversityId == universityId && s.IsCurrent);
        }

        public async Task<SemesterAuthorizationInfo?> GetSemesterAuthorizationInfoAsync(int semesterId)
        {
            return await _context.Semesters.Where(s => s.SemesterId == semesterId).Select(ToInfo).SingleOrDefaultAsync();
        }

        public async Task<bool> IsExistsById(int semesterId)
        {
            return await _context.Semesters.AnyAsync(s => s.SemesterId == semesterId);
        }

        public async Task<bool> IsExistsByName(int universityId, string name)
        {
            return await _context.Semesters.AnyAsync(s => s.UniversityId == universityId && s.Name == name);
        }
    }
}
