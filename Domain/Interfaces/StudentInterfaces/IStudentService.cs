using Contracts.Common.AuthorizationInfos.StudentAuthorizationInfo;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudentRequests;
using Contracts.Responses;
using Contracts.Responses.StudentResponses;
using Contracts.Results;
using Domain.Entities.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.StudentInterfaces
{
    public interface IStudentService
    {
        public Task<PagedResult<StudentDTO>> GetStudents(UserScope? scope, StudentFilter? filter, int pageNumber, int pageSize);
        public Task<StudentDTO?> GetDTOById(int studentId);
        public Task<StudentDTO?>GetDTOByStudentNumber(string studentNumber);
        public Task<Student?>GetEntityById(int studentId);
        public Task<bool>ExistsByStudentNumber(string studentNumber);
        public Task<AddUpdateServiceResponse<StudentDTO>> AddStudent(UserScope?scope,AddStudentDTO newStudent, int currentUser);
        public Task<AddUpdateServiceResponse<StudentDTO>> UpdateStudent(int studentId,UserScope?scope,UpdateStudentDTO updatedStudent, int currentUser);
        public Task<AddUpdateServiceResponse<StudentDTO>>AddBatchAdmin(UserScope? scope, AddStudentDTO newStudent, int currentUser);
        public Task<AddUpdateServiceResponse<StudentDTO>> UpdateBatchAdmin(int studentId,UserScope? scope, UpdateStudentDTO updatedStudent, int currentUser);
        public Task<StudentAuthorizationInfo?> GetStudentAuthorizationInfoAsync(int studentId);
    }
}
