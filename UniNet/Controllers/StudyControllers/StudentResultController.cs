using Contracts.Common.Messages;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.StudentResultRequests;
using Contracts.Responses;
using Contracts.Responses.StudyResponses.StudentResultResponses;
using Contracts.Results;
using Domain.Entities.Students;
using Domain.Entities.Study;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Domain.Interfaces.StudyInterfaces.StudentResultInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Extensions;

namespace UniNet.Controllers.StudyControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentResultController : ControllerBase
    {
        private readonly IStudentResultService _studentResultService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthorizationService _authorizationService;
        public StudentResultController(IStudentResultService studentResultService, ICurrentUserService currentUserService, IAuthorizationService authorizationService)
        {
            _studentResultService = studentResultService;
            _currentUserService = currentUserService;
            _authorizationService = authorizationService;
        }

        [Authorize(Roles = "Super Admin")]
        [HttpGet(Name = "GetAllStudentResults")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<DetaieldStudentResultDTO>>> GetAllStudentResults([FromQuery] StudentResultFilterDTO? filter, [FromQuery] PagedResultParameters @pagedResultParameters)
        {
            var results = await _studentResultService.GetAll(filter, pagedResultParameters.PageNumber, pagedResultParameters.PageSize);
            return results.ToPagedActioneResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin,BatchAdmin")]
        [HttpGet("by-id", Name = "GetStudentResultById")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetaieldStudentResultDTO>> GetStudentResultById([FromQuery] StudentResultIdParameter @studentResultIdParameter)
        {
            var info = await _studentResultService.GetStudentResultAuthorizationInfoAsync(studentResultIdParameter.StudentResultId);
            if (info == null)
                return NotFound(ErrorMessages.NotFound<StudentResult>(studentResultIdParameter.StudentResultId));

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, info, "StudentResultOwnerPolicy");
            if (!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var result = await _studentResultService.GetDetaieldStudentResultDTOById(studentResultIdParameter.StudentResultId);
            return result.GetResourceEndpoints(studentResultIdParameter.StudentResultId, typeof(StudentResult).Name);
        }

        // كشف درجات طالب واحد + المعدّل، مرتّب حسب SemesterId. (يستطيع الطالب رؤية نتائجه فقط)
        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin,BatchAdmin,Student")]
        [HttpGet("student", Name = "GetStudentResultsPerStudent")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<StudentSemesterResultDTO>>> GetStudentResultsPerStudent([FromQuery] StudentIdParameter @studentIdParameter, [FromQuery] StudentResultFilterDTO? filter)
        {
            var studentInfo = await _studentResultService.GetStudentAuthorizationInfoAsync(studentIdParameter.StudentId);
            if (studentInfo == null)
                return NotFound(ErrorMessages.NotFound<Student>(studentIdParameter.StudentId));

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, studentInfo, "StudentOwnerPolicy");
            if (!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var results = await _studentResultService.GetStudentResults(studentIdParameter.StudentId, filter);
            return Ok(results);
        }

        // تقرير درجات كل الطلاب ضمن النطاق، مرتّب حسب Section => Batch => Department => College => University.
        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin,BatchAdmin")]
        [HttpGet("report", Name = "GetAllStudentsResultsReport")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<StudentSemesterResultDTO>>> GetAllStudentsResultsReport([FromQuery] StudentResultFilterDTO? filter)
        {
            var results = await _studentResultService.GetAllStudentsResults(_currentUserService.ToUserScope(), filter);
            return Ok(results);
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin,BatchAdmin")]
        [HttpPost(Name = "AddStudentResult")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AddUpdateServiceResponse<DetaieldStudentResultDTO>>> AddStudentResult([FromBody] AddStudentResultDTO newStudentResult)
        {
            var response = await _studentResultService.AddStudentResult(_currentUserService.ToUserScope(), newStudentResult, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin,BatchAdmin")]
        [HttpPut(Name = "UpdateStudentResult")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<DetaieldStudentResultDTO>>> UpdateStudentResult([FromQuery] StudentResultIdParameter @studentResultIdParameter, [FromBody] UpdateStudentResultDTO updatedStudentResult)
        {
            var response = await _studentResultService.UpdateStudentResult(_currentUserService.ToUserScope(), updatedStudentResult, studentResultIdParameter.StudentResultId, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin,BatchAdmin")]
        [HttpDelete(Name = "DeleteStudentResult")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> DeleteStudentResult([FromQuery] StudentResultIdParameter @studentResultIdParameter)
        {
            var info = await _studentResultService.GetStudentResultAuthorizationInfoAsync(studentResultIdParameter.StudentResultId);
            if (info == null)
                return NotFound(ErrorMessages.NotFound<StudentResult>(studentResultIdParameter.StudentResultId));

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, info, "StudentResultOwnerPolicy");
            if (!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var result = await _studentResultService.Delete(studentResultIdParameter.StudentResultId);
            return result.ToDeleteActionResult<StudentResult>(studentResultIdParameter.StudentResultId);
        }
    }
}
