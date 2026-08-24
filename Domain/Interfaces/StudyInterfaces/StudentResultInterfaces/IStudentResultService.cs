using Contracts.Common.AuthorizationInfos.StudentAuthorizationInfo;
using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.StudentResultRequests;
using Contracts.Responses;
using Contracts.Responses.StudyResponses.StudentResultResponses;
using Contracts.Results;
using Domain.Entities.Study;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.StudyInterfaces.StudentResultInterfaces
{
    public interface IStudentResultService
    {
        public Task<PagedResult<DetaieldStudentResultDTO>> GetAll(StudentResultFilterDTO? filter, int pageNumber, int pageSize);
        public Task<DetaieldStudentResultDTO?> GetDetaieldStudentResultDTOById(int studentResultId);
        public Task<StudentResult?> GetEntityById(int studentResultId);
        public Task<StudentResultAuthorizationInfo?> GetStudentResultAuthorizationInfoAsync(int studentResultId);
        public Task<StudentAuthorizationInfo?> GetStudentAuthorizationInfoAsync(int studentId);
        public Task<bool> IsExistsById(int studentResultId);
        public Task<bool> Delete(int studentResultId);
        public Task<AddUpdateServiceResponse<DetaieldStudentResultDTO>> AddStudentResult(UserScope? scope, AddStudentResultDTO newStudentResult, int currentUserId);
        public Task<AddUpdateServiceResponse<DetaieldStudentResultDTO>> UpdateStudentResult(UserScope? scope, UpdateStudentResultDTO updatedStudentResult, int studentResultId, int currentUserId);

        // كشف درجات طالب واحد + المعدّل، مرتّب حسب SemesterId.
        public Task<List<StudentSemesterResultDTO>> GetStudentResults(int studentId, StudentResultFilterDTO? filter);

        // تقرير كل الطلاب، مرتّب حسب Section => Batch => Department => College => University.
        public Task<List<StudentSemesterResultDTO>> GetAllStudentsResults(UserScope? scope, StudentResultFilterDTO? filter);
    }
}
