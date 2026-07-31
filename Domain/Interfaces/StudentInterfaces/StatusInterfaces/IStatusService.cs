using Contracts.Requests.StudentRequests;
using Contracts.Responses;
using Contracts.Responses.AcademicResponses.StudentResponses;
using Contracts.Results;
using Domain.Entities.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.StudentInterfaces.StatusInterfaces
{
    public interface IStatusService
    {
        public Task<PagedResult<StudentStatusDTO>> GetStatuses(int pageNumber, int pageSize);
        public Task<StudentStatusDTO?> GetDTOById(int statusId);
        public Task<StudentStatusDTO?> GetDTOByName(string Name);
        public Task<StudentStatus?> GetEntityById(int statusId);
        public Task<AddUpdateServiceResponse<StudentStatusDTO>> AddStatus(AddUpdateStudentStatusDTO newStatus, int currentUserId);
        public Task<AddUpdateServiceResponse<StudentStatusDTO>>UpdateStatus(int statusId,AddUpdateStudentStatusDTO updatedStatus, int currentUserId);
        public Task<bool> IsExistsByName(string name);
    }
}
