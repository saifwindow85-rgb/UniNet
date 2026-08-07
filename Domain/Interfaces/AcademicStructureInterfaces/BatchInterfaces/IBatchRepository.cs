using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Responses.AcademicResponses.BatchResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Contracts.Requests.AcademicRequests.CommonAcademicRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Requests.RequestParameters;

namespace Domain.Interfaces.AcademicStructureInterfaces.BatchInterfaces
{
    public interface IBatchRepository
    {
        public Task<PagedResult<BatchDTO>> GetAllBatches(AcademicFilter?filter,int pageNumber, int pageSize);
        public Task<PagedResult<BatchDTO>>GetBatchesPerDepartment(UserScope?scope,AcademicFilter?filter,int pageNumber, int pageSize);
        public Task<BatchDTO?> GetDTOById(int batchId);
        public Task<Batch?>GetEntityById(int batchId);
        public Task<bool> Delete(int batchId);
        public Task<bool>ExistsById(int batchId);
        public Task<bool>ExistsByName(int departmentId,string name);
        public void Add(Batch batch);
        public Task<BatchAuthorizationInfo?> GetBatchAuthorizationInfoAsync(int batchId);
    }
}
