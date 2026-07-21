using Contracts.Requests.AcademicRequests.BatchRequests;
using Contracts.Responses;
using Contracts.Responses.AcademicResponses.BatchResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.AcademicStructureInterfaces.BatchInterfaces
{
    public interface IBatchService
    {
        public Task<PagedResult<BatchDTO>> GetAllBatches(int pageNumber, int pageSize);
        public Task<PagedResult<BatchDTO>> GetBatchesPerDepartment(int departmentId, int pageNumber, int pageSize);
        public Task<BatchDTO?> GetDTOById(int batchId);
        public Task<Batch?> GetEntityById(int batchId);
        public Task<BatchDTO?> GetDTOByName(int departmentId, string name);
        public Task<Batch?> GetEntityByName(int departmentId, string name);
        public Task<bool> Delete(int batchId);
        public Task<bool> ExistsById(int batchId);
        public Task<bool> ExistsByName(int departmentId, string name);
        public Task<AddUpdateServiceResponse<BatchDTO>> AddBatch(AddBatchDTO newBatch, int currentUserId);
        public Task<AddUpdateServiceResponse<BatchDTO>> UpdateBatch(int batchId, UpdateBatchDTO updatedBatch, int currentUserId);
    }
}
