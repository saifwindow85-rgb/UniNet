using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Requests.AcademicRequests.CommonAcademicRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses.AcademicResponses.SectionResponses;
using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Extensions;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.SectionInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos.AcademicRepositories
{
    public class SectionRepository : ISectionRepository
    {
        private AppDbcontext _context;
        private readonly Expression<Func<Section, SectionDTO>> ToDTO = s => new SectionDTO
        {
            SectionId = s.SectionId,
            SectionName = s.Name,
            BatchId = s.BatchId,
            BatchName = s.Batch.Name,
            CreatedAt = s.CreatedAt,
            CreatedByUserId = s.CreatedByUserId,
            CreatedByUserName = s.CreatedByUser.UserName,
            UpdatedAt = s.UpdatedAt,
            UpdatedByUserId = s.UpdatedByUserId,
            UpdatedByUserName = s.UpdatedByUser == null ? null : s.UpdatedByUser.UserName,
        };

        private readonly Expression<Func<Section, SectionAuthorizationInfo>> ToInfo = s => new SectionAuthorizationInfo
        {
            UniversityId = s.Batch.Department.College.UniversityId,
            CollegeId = s.Batch.Department.CollegeId,
            DepartmentId = s.Batch.DepartmentId,
            SectionId = s.SectionId,
            BatchId = s.BatchId,
        };

        public SectionRepository(AppDbcontext context)
        {
            _context = context;
        }

        public void Add(Section section)
        {
            _context.Sections.Add(section);
        }

        public async Task<bool> Delete(int sectionId)
        {
            var section = await _context.Sections.FindAsync(sectionId);
            if (section == null)
                return false;

            _context.Sections.Remove(section);
            return true;
        }

        public async Task<bool> ExistsById(int sectionId)
        {
           return await _context.Sections.AnyAsync(s=>s.SectionId ==  sectionId);
        }

        public async Task<bool> ExistsByName(int batchId, string name)
        {
            return await _context.Sections.Where(s => s.BatchId == batchId).AnyAsync(s => s.Name == name);
        }

        public async Task<PagedResult<SectionDTO>> GetAllSections(AcademicFilter?filter,int pageNumber, int pageSize)
        {
            if(filter == null)
                filter = new AcademicFilter();

            var query = _context.Sections.AsNoTracking().OrderBy(s => s.BatchId).ThenBy(s => s.SectionId).AsQueryable();

            if(filter.UniversityId.HasValue)
            {
                query = query.Where(s => s.Batch.Department.College.UniversityId == filter.UniversityId);
            }

            if(filter.CollegeId.HasValue)
            {
                query = query.Where(s => s.Batch.Department.CollegeId == filter.CollegeId);
            }

            if(filter.DepartmentId.HasValue)
            {
                query = query.Where(s=>s.Batch.DepartmentId == filter.DepartmentId);
            }

            if(filter.BatchId.HasValue)
            {
                query = query.Where(s=>s.BatchId == filter.BatchId);
            }

            if(filter.SectionId.HasValue)
            {
                query = query.Where(s=>s.SectionId == filter.SectionId);
            }

            if(!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(s => EF.Functions.Like(s.Name, $"%{filter.Search}%"));
            }

            if(filter.StartDate.HasValue)
            {
                query = query.Where(s=>s.CreatedAt >=  filter.StartDate);
            }

            if(filter.EndDate.HasValue)
            {
                query = query.Where(s=>s.CreatedAt <= filter.EndDate);
            }

            return await query.Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }



        public async Task<SectionDTO?> GetDTOById(int sectionId)
        {
            return await _context.Sections.AsNoTracking().Select(ToDTO).SingleOrDefaultAsync(s => s.SectionId == sectionId);
        }
  
        public async Task<Section?> GetEntityById(int sectionId)
        {
            return await _context.Sections.FindAsync(sectionId);
        }


        public async Task<PagedResult<SectionDTO>> GetSectionsPerBatch(UserScope?scope,AcademicFilter?filter, int pageNumber, int pageSize)
        {
            if (filter == null)
                filter = new AcademicFilter();

            if (scope == null || (scope.UniversityId == null && scope.CollegeId == null && scope.DepartmentId == null && scope.BatchId == null))
                scope = new UserScope();

            var query = _context.Sections.AsNoTracking().OrderBy(s => s.BatchId).ThenBy(s => s.SectionId).AsQueryable();

            if (scope.UniversityId.HasValue)
            {
                query = query.Where(s => s.Batch.Department.College.UniversityId == scope.UniversityId);
            }

            else if (filter.UniversityId.HasValue)
            {
                query = query.Where(s => s.Batch.Department.College.UniversityId == filter.UniversityId);
            }

            if (scope.CollegeId.HasValue)
            {
                query = query.Where(s => s.Batch.Department.CollegeId == scope.CollegeId);
            }

            else if (filter.CollegeId.HasValue)
            {
                query = query.Where(s => s.Batch.Department.CollegeId == filter.CollegeId);
            }

            if (scope.DepartmentId.HasValue)
            {
                query = query.Where(s => s.Batch.DepartmentId == scope.DepartmentId);
            }

            else if (filter.DepartmentId.HasValue)
            {
                query = query.Where(s => s.Batch.DepartmentId == filter.DepartmentId);
            }

            if(scope.BatchId.HasValue)
            {
                query = query.Where(s => s.BatchId == scope.BatchId);
            }
            else if(filter.BatchId.HasValue)
            {
                query = query.Where(s => s.BatchId == filter.BatchId);
            }

            if (!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(s => EF.Functions.Like(s.Name, $"%{filter.Search}%"));
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(s => s.CreatedAt >= filter.StartDate);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(s => s.CreatedAt <= filter.EndDate);
            }

            return await query.Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<SectionAuthorizationInfo?> GetSectionAuthorizationInfoAsync(int sectionId)
        {
            return await _context.Sections.Where(s => s.SectionId == sectionId).Select(ToInfo).SingleOrDefaultAsync();
        }
    }
}
