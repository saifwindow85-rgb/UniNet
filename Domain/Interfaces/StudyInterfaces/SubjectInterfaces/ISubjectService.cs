using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.SubjectRequests;
using Contracts.Responses;
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
    public interface ISubjectService
    {
        public Task<PagedResult<SubjectDTO>> GetAll(SubjectFilterDTO? filter, int pageNumber, int pageSize);
        public Task<PagedResult<SubjectDTO>> GetSubjectsPerDepartments(UserScope? scope, SubjectFilterDTO? filter, int pageNumber, int pageSize);
        public Task<SubjectDTO?> GetDTOById(int subjectId);
        public Task<Subject?> GetEntityById(int subjectId);
        public Task<bool> Delete(int subjectId);
        public Task<bool> IsExistsById(int subjectId);
        public Task<bool> IsExistsByName(int departmentId, string name);
        public Task<AddUpdateServiceResponse<SubjectDTO>> AddSubject(UserScope? scope, AddSubjectDTO newSubject, int currentUserId);
        public Task<AddUpdateServiceResponse<SubjectDTO>> UpdateSubject(UserScope? scope, UpdateSubjectDTO updatedSubject, int subjectId, int currentUserId);
        public Task<SubjectAuthorizationInfo?> GetSubjectAuthorizationInfoAsync(int subjecId);
        public Task<DetaieldSubjectDTO?> GetDetaieldSubjectDTOById(int subjectId);
    }
}
