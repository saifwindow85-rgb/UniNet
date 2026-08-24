using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.SemesterRequests;
using Contracts.Responses.StudyResponses.SemesterResponses;
using Contracts.Results;
using Domain.Entities.Study;
using System.Threading.Tasks;

namespace Domain.Interfaces.StudyInterfaces.SemesterInterfaces
{
    public interface ISemesterRepository
    {
        public void Add(Semester semester);
        public bool Delete(Semester semester);
        public Task<SemesterDTO?> GetDTOById(int semesterId);
        public Task<DetaieldSemesterDTO?> GetDetaieldSemesterDTOById(int semesterId);
        public Task<Semester?> GetEntityById(int semesterId);
        public Task<PagedResult<SemesterDTO>> GetAll(SemesterFilterDTO? filter, int pageNumber, int pageSize);
        public Task<PagedResult<SemesterDTO>> GetSemestersPerUniversity(UserScope? scope, SemesterFilterDTO? filter, int pageNumber, int pageSize);
        public Task<Semester?> GetCurrentSemesterEntity(int universityId);
        public Task<SemesterAuthorizationInfo?> GetSemesterAuthorizationInfoAsync(int semesterId);
        public Task<bool> IsExistsById(int semesterId);
        public Task<bool> IsExistsByName(int universityId, string name);
    }
}
