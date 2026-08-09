using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Extensions;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.CollegeInterfaces;
using Microsoft.EntityFrameworkCore;
using Contracts.Common.AuthorizationInfos.AcademicInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Contracts.Responses.AcademicResponses.CollegeResponses;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.AcademicRequests.CommonAcademicRequests;

namespace DataAccessLayer.Repos.AcademicRepositories
{
    public class CollegeRepository : ICollegeRepository
    {
        private Expression<Func<College, CollegeDTO>> ToDTO = c => new CollegeDTO
        {
            CollegeId = c.CollegeId,
            CollegeName = c.Name,
            Description = c.Description,
            UniversityId = c.UniversityId,
            UniversityName = c.University.Name,
            CreatedAt = c.CreatedAt,
            CreatedByUserId = c.CreatedByUserId,
            CreatedByUserName = c.CreatedByUser.UserName,
            UpdatedAt = c.UpdatedAt,
            UpdatedByUserId = c.UpdatedByUserId,
            UpdatedByUserName = c.UpdatedByUser == null ? null : c.UpdatedByUser.UserName,
        };

        private readonly Expression<Func<College, CollegeAuthorizationInfo>> ToInfo = c => new CollegeAuthorizationInfo
        {
            CollegeId = c.CollegeId,
            UniversityId = c.UniversityId,
        };
        private readonly AppDbcontext _context;
        public CollegeRepository(AppDbcontext context)
        {
            _context = context;
        }
        public void Add(College college)
        {
            _context.Colleges.Add(college);
        }

        public async Task<bool> Delete(int collegeId)
        {
         
            var college = await _context.Colleges.FindAsync(collegeId);
            if (college == null)
                return false;
            _context.Colleges.Remove(college);
            return true;
        }

        public async Task<PagedResult<CollegeDTO>> GetAllColleges(AcademicFilter? filter,int pageNumber, int pageSize)
        {
            var query = _context.Colleges.AsNoTracking().OrderBy(c=>c.UniversityId).ThenBy(c=>c.CollegeId).AsQueryable();
            if(filter == null)
                filter = new AcademicFilter();

            if (!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(c =>EF.Functions.Like(c.Name,$"%{filter.Search}%"));
            }

            if(filter.UniversityId.HasValue)
            {
                query = query.Where(c => c.UniversityId == filter.UniversityId.Value);
            }

            if(filter.CollegeId.HasValue)
            {
                query = query.Where(c => c.CollegeId == filter.CollegeId);
            }

            if(filter.StartDate.HasValue)
            {
                query = query.Where(c => c.CreatedAt >= filter.StartDate);
            }

            if(filter.EndDate.HasValue)
            {
                query = query.Where(c => c.CreatedAt <= filter.EndDate);
            }
            return await query.Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<PagedResult<CollegeDTO>> GetAllCollegesPerUniversity(UserScope?scope,AcademicFilter?filter, int pageNumber, int pageSize)
        {

            if (scope == null || (scope.UniversityId == null && scope.CollegeId == null && scope.DepartmentId == null && scope.BatchId == null))
                scope = new UserScope();

            if (filter == null)
                filter = new AcademicFilter();

            var query = _context.Colleges.AsNoTracking().OrderBy(c => c.UniversityId).ThenBy(c=>c.CollegeId).AsQueryable();

            if(scope.UniversityId.HasValue)
            {
                query = query.Where(c => c.UniversityId == scope.UniversityId.Value);
            }
            else if(filter.UniversityId.HasValue)
            {
                query = query.Where(c => c.UniversityId == filter.UniversityId);
            }


            if(!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(c => EF.Functions.Like(c.Name, $"%{filter.Search}%"));
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(c => c.CreatedAt >= filter.StartDate);
            }

            if(filter.EndDate.HasValue)
            {
                query = query.Where(c=>c.CreatedAt <= filter.EndDate);
            }

            return await query.Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<CollegeDTO?> GetCollegeDTOById(int collegeId)
        {
            return await _context.Colleges.AsNoTracking().Select(ToDTO).SingleOrDefaultAsync(c => c.CollegeId == collegeId);
        }

        public async Task<College?> GetCollegeEntityById(int collegeId)
        {
            return await _context.Colleges.FindAsync(collegeId);
        }

        public async Task<bool> IsCollegeExists(int collegeId)
        {
            return await _context.Colleges.AnyAsync(c => c.CollegeId == collegeId);
        }

        public async Task<bool> IsCollegeExists(int universityId,string collegeName)
        {
            return await _context.Colleges.AnyAsync(c => c.Name == collegeName && c.UniversityId == universityId);
        }


        public async Task<CollegeAuthorizationInfo?> GetCollegeAuthorizationInfo(int collegeId)
        {
            return await _context.Colleges.AsNoTracking().Select(ToInfo).SingleOrDefaultAsync(c => c.CollegeId == collegeId);
        }

       
    }
}
