using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.SubjectRequests;
using Contracts.Responses.StudyResponses.SubjectResponses;
using Contracts.Results;
using Domain.Entities.Study;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.StudyInterfaces.SubjectInterfaces
{
    public interface ISubjectRepository
    {
        public void Add(Subject subject);
        public Task<SubjectDTO?> GetDTOById(int subjectId);
        public Task<Subject?> GetEntityById(int subjectId);
        public Task<PagedResult<SubjectDTO>> GetAll(SubjectFilterDTO?filter,int pageNumber,int pageSize);
        public Task<PagedResult<SubjectDTO>> GetSubjectsPerDepartment(UserScope? scope, SubjectFilterDTO? filter, int pageNumber, int pageSize);
        public bool Delete(Subject subject);
        public Task<bool> IsExistsById(int subjectId);
        public Task<bool> IsExistsByName(int departmentId,string name);
        public Task<SubjectAuthorizationInfo?> GetSubjectAuthorizationInfoAsync(int subjecId);
        public Task<DetaieldSubjectDTO?> GetDetaieldSubjectDTOById(int subjectId);
    }
}
