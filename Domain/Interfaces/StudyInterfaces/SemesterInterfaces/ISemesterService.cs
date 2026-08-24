using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.SemesterRequests;
using Contracts.Responses;
using Contracts.Responses.StudyResponses.SemesterResponses;
using Contracts.Results;
using Domain.Entities.Study;
using System.Threading.Tasks;

namespace Domain.Interfaces.StudyInterfaces.SemesterInterfaces
{
    public interface ISemesterService
    {
        public Task<PagedResult<SemesterDTO>> GetAll(SemesterFilterDTO? filter, int pageNumber, int pageSize);
        public Task<PagedResult<SemesterDTO>> GetSemestersPerUniversity(UserScope? scope, SemesterFilterDTO? filter, int pageNumber, int pageSize);
        public Task<SemesterDTO?> GetDTOById(int semesterId);
        public Task<DetaieldSemesterDTO?> GetDetaieldSemesterDTOById(int semesterId);
        public Task<Semester?> GetEntityById(int semesterId);
        public Task<SemesterDTO?> GetCurrentSemester(int universityId);
        public Task<SemesterAuthorizationInfo?> GetSemesterAuthorizationInfoAsync(int semesterId);
        public Task<bool> IsExistsById(int semesterId);
        public Task<bool> IsExistsByName(int universityId, string name);
        public Task<bool> Delete(int semesterId);
        public Task<AddUpdateServiceResponse<SemesterDTO>> AddSemester(UserScope? scope, AddSemesterDTO newSemester, int currentUserId);
        public Task<AddUpdateServiceResponse<SemesterDTO>> UpdateSemester(UserScope? scope, UpdateSemesterDTO updatedSemester, int semesterId, int currentUserId);
        public Task<AddUpdateServiceResponse<SemesterDTO>> EndSemester(UserScope? scope, int semesterId, int currentUserId);
    }
}
