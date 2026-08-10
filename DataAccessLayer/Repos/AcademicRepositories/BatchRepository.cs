using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Requests.AcademicRequests.CommonAcademicRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses.AcademicResponses.BatchResponses;
using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Extensions;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.BatchInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos.AcademicRepositories
{
    public class BatchRepository : IBatchRepository
    {
        private readonly AppDbcontext _context;
        private readonly Expression<Func<Batch, BatchDTO>> ToDTO = b => new BatchDTO
        {
            BatchId = b.BatchId,
            BatchName = b.Name,
            DepartmentId = b.DepartmentId,
            DepartmentName = b.Department.Name,
            BatchYear = b.BatchYear,
            Description = b.Description,
            CreatedAt = b.CreatedAt,
            CreatedByUserId = b.CreatedByUserId,
            CreatedByUserName = b.CreatedByUser.UserName,
            UpdatedAt = b.UpdatedAt,
            UpdatedByUserId = b.UpdatedByUserId,
            UpdatedByUserName = b.UpdatedByUser == null ? null : b.UpdatedByUser.UserName,
        };
        private readonly Expression<Func<Batch, BatchAuthorizationInfo>> ToInfo = b => new BatchAuthorizationInfo
        {
            UniversityId = b.Department.College.UniversityId,
            CollegeId = b.Department.CollegeId,
            DepartmentId = b.DepartmentId,
            BatchId = b.BatchId,
        };
        public BatchRepository(AppDbcontext context)
        {
            _context = context;
        }

        public void Add(Batch batch)
        {
             _context.Batches.Add(batch);
        }

        public async Task<bool> Delete(int batchId)
        {
            var batch = await _context.Batches.FindAsync(batchId);
            if (batch == null)
                return false;
            _context.Remove(batch);
            return true;
        }

        public async Task<bool> ExistsById(int batchId)
        {
            return await _context.Batches.AnyAsync(b=>b.BatchId == batchId);
        }

        public async Task<bool> ExistsByName(int departmentId, string name)
        {
            return await _context.Batches.AnyAsync(b => b.DepartmentId == departmentId && b.Name == name);
        }

        public async Task<PagedResult<BatchDTO>> GetAllBatches(AcademicFilter?filter,int pageNumber, int pageSize)
        {
           if(filter == null)
                filter = new AcademicFilter();

            var query = _context.Batches.AsNoTracking().OrderBy(b => b.Department.College.UniversityId).ThenBy(b => b.Department.CollegeId)
                 .ThenBy(b => b.DepartmentId).ThenBy(b => b.BatchId).AsQueryable();

            if(filter.UniversityId.HasValue)
            {
                query = query.Where(b=>b.Department.College.UniversityId == filter.UniversityId);
            }

            if(filter.CollegeId.HasValue)
            {
                query = query.Where(b => b.Department.CollegeId == filter.CollegeId);
            }

            if(filter.DepartmentId.HasValue)
            {
                query = query.Where(b=>b.DepartmentId == filter.DepartmentId);
            }

            if(filter.BatchId.HasValue)
            {
                query = query.Where(b => b.BatchId == filter.BatchId);
            }

            if(!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(b => EF.Functions.Like(b.Name, $"%{filter.Search}%"));
            }

            if(filter.StartDate.HasValue)
            {
                query = query.Where(b=>b.CreatedAt >= filter.StartDate);
            }

            if(filter.EndDate.HasValue)
            {
                query = query.Where(b=>b.CreatedAt <= filter.EndDate);
            }

            return await query.Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<BatchAuthorizationInfo?> GetBatchAuthorizationInfoAsync(int batchId)
        {
            return await _context.Batches.Where(b => b.BatchId == batchId).Select(ToInfo).SingleOrDefaultAsync();
        }

        public async Task<PagedResult<BatchDTO>> GetBatchesPerDepartment(UserScope?scope,AcademicFilter?filter, int pageNumber, int pageSize)
        {
           if(filter == null)
                filter = new AcademicFilter();

            if (scope == null || (scope.UniversityId == null && scope.CollegeId == null && scope.DepartmentId == null && scope.BatchId == null))
                scope = new UserScope();

            var query = _context.Batches.AsNoTracking().OrderBy(b => b.Department.College.UniversityId).ThenBy(b => b.Department.CollegeId)
                       .ThenBy(b => b.DepartmentId).ThenBy(b => b.BatchId).AsQueryable();

            if(scope.UniversityId.HasValue)
            {
                query = query.Where(b => b.Department.College.UniversityId == scope.UniversityId);
            }

            else if(filter.UniversityId.HasValue)
            {
                query = query.Where(b => b.Department.College.UniversityId == filter.UniversityId);
            }

            if(scope.CollegeId.HasValue)
            {
                query = query.Where(b => b.Department.CollegeId == scope.CollegeId);
            }

            else if(filter.CollegeId.HasValue)
            {
                query = query.Where(b => b.Department.CollegeId == filter.CollegeId);
            }

            if(scope.DepartmentId.HasValue)
            {
                query = query.Where(b => b.DepartmentId == scope.DepartmentId);
            }

            else if(filter.DepartmentId.HasValue)
            {
                query = query.Where(b => b.DepartmentId == filter.DepartmentId);
            }
            

            if(!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(b => EF.Functions.Like(b.Name, $"%{filter.Search}%"));
            }

            if(filter.StartDate.HasValue)
            {
                query = query.Where(b => b.CreatedAt >= filter.StartDate);
            }

            if(filter.EndDate.HasValue)
            {
                query = query.Where(b => b.CreatedAt <= filter.EndDate);
            }

            return await query.Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<BatchDTO?> GetDTOById(int batchId)
        {
            return await _context.Batches.Select(ToDTO).SingleOrDefaultAsync(b=>b.BatchId==batchId);
        }


        public async Task<Batch?> GetEntityById(int batchId)
        {
            return await _context.Batches.FindAsync(batchId);
        }

    }
}
