using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.SectionSubjectRequests;
using Contracts.Responses.StudyResponses.SectionSubjectResponses;
using Contracts.Results;
using Domain.Entities.Study;
using System.Threading.Tasks;

namespace Domain.Interfaces.StudyInterfaces.SectionSubjectInterfaces
{
    public interface ISectionSubjectRepository
    {
        public void Add(SectionSubject sectionSubject);
        public bool Delete(SectionSubject sectionSubject);
        public Task<SectionSubjectDTO?> GetDTOById(int sectionSubjectId);
        public Task<DetaieldSectionSubjectDTO?> GetDetaieldSectionSubjectDTOById(int sectionSubjectId);
        public Task<SectionSubject?> GetEntityById(int sectionSubjectId);
        public Task<PagedResult<SectionSubjectDTO>> GetAll(SectionSubjectFilterDTO? filter, int pageNumber, int pageSize);
        public Task<PagedResult<SectionSubjectDTO>> GetSectionSubjectsPerScope(UserScope? scope, SectionSubjectFilterDTO? filter, int pageNumber, int pageSize);
        public Task<SectionSubjectAuthorizationInfo?> GetSectionSubjectAuthorizationInfoAsync(int sectionSubjectId);
        public Task<bool> IsExistsById(int sectionSubjectId);
        public Task<bool> IsAlreadyAssigned(int sectionId, int subjectId, int semesterId);
    }
}
