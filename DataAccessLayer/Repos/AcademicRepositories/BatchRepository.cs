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

        public async Task<PagedResult<BatchDTO>> GetAllBatches(int pageNumber, int pageSize)
        {
            return await _context.Batches.AsNoTracking().OrderBy(b=>b.DepartmentId).Select(ToDTO)
                .ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<PagedResult<BatchDTO>> GetBatchesPerDepartment(int departmentId, int pageNumber, int pageSize)
        {
            return await _context.Batches.AsNoTracking()
                .Where(b=>b.DepartmentId == departmentId).OrderBy(b => b.DepartmentId).Select(ToDTO)
                        .ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<BatchDTO?> GetDTOById(int batchId)
        {
            return await _context.Batches.Select(ToDTO).SingleOrDefaultAsync(b=>b.BatchId==batchId);
        }

        public async Task<BatchDTO?> GetDTOByName(int departmentId, string name)
        {
            return await _context.Batches.Where(b => b.DepartmentId == departmentId && b.Name == name).Select(ToDTO).SingleOrDefaultAsync();
        }

        public async Task<Batch?> GetEntityById(int batchId)
        {
            return await _context.Batches.FindAsync(batchId);
        }

        public async Task<Batch?> GetEntityByName(int departmentId, string name)
        {
            return await _context.Batches.Where(b=>b.DepartmentId == departmentId && b.Name == name).SingleOrDefaultAsync();
        }
    }
}
