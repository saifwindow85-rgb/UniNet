using Contracts.Common.AuthorizationInfos.StudentAuthorizationInfo;
using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.StudentResultRequests;
using Contracts.Responses.StudyResponses.StudentResultResponses;
using Contracts.Results;
using Domain.Entities.Study;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.StudyInterfaces.StudentResultInterfaces
{
    public interface IStudentResultRepository
    {
        public void Add(StudentResult studentResult);
        public bool Delete(StudentResult studentResult);
        public Task<StudentResult?> GetEntityById(int studentResultId);
        public Task<DetaieldStudentResultDTO?> GetDetaieldStudentResultDTOById(int studentResultId);
        public Task<PagedResult<DetaieldStudentResultDTO>> GetAll(StudentResultFilterDTO? filter, int pageNumber, int pageSize);
        public Task<StudentResultAuthorizationInfo?> GetStudentResultAuthorizationInfoAsync(int studentResultId);
        public Task<StudentAuthorizationInfo?> GetStudentAuthorizationInfoAsync(int studentId);
        public Task<bool> IsExistsById(int studentResultId);
        public Task<bool> IsAlreadyRecorded(int studentId, int sectionSubjectId);

        // كشف درجات طالب واحد، مجموعة حسب الفصل ومرتّبة حسب SemesterId.
        public Task<List<StudentSemesterResultDTO>> GetStudentResults(int studentId, StudentResultFilterDTO? filter);

        // تقرير كل الطلاب، مرتّب حسب Section => Batch => Department => College => University.
        public Task<List<StudentSemesterResultDTO>> GetAllStudentsResults(UserScope? scope, StudentResultFilterDTO? filter);
    }
}
