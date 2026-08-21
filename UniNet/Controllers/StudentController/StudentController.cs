using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudentRequests;
using Contracts.Responses.StudentResponses;
using Contracts.Results;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Domain.Interfaces.StudentInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Extensions;
using Contracts.Common.Messages;
using Domain.Entities.Students;
using Contracts.Responses;

namespace UniNet.Controllers.StudentController
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IStudentService _studentService;
        private readonly IAuthorizationService _authorizationService;
        public StudentController(ICurrentUserService currentUserService, IStudentService studentService,IAuthorizationService authorizationService)
        {
            _currentUserService = currentUserService;
            _studentService = studentService;
            _authorizationService = authorizationService;
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin,BatchAdmin")]
        [HttpGet(Name ="GetStudents")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<StudentDTO>>> GetStudents([FromQuery] StudentFilter? filter, [FromQuery] PagedResultParameters pagedResultParameters)
        {
            var students = await _studentService.GetStudents(_currentUserService.ToUserScope(),filter,pagedResultParameters.PageNumber, pagedResultParameters.PageSize);
            return students.ToPagedActioneResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin,BatchAdmin,Student")]
        [HttpGet("by_id",Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentDTO>> GetStudentById([FromQuery]IdParameter idParameter)
        {
            var studentInfo = await _studentService.GetStudentAuthorizationInfoAsync(idParameter.Id);
            if (studentInfo == null)
                return NotFound(ErrorMessages.NotFound<Student>(idParameter.Id));

            var authorization = await _authorizationService.AuthorizeAsync(User, studentInfo, "StudentOwnerPolicy");
            if (!authorization.Succeeded)
                return authorization.Succeeded.NotAuthorized();

            var student = await _studentService.GetDTOById(idParameter.Id);
            return student.GetResourceEndpoints(idParameter.Id, typeof(Student).Name);
        }


        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpPost("batch_admin", Name = "CreateBatchAdmin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<StudentDTO>>> AddBatchAdmin([FromBody]AddStudentDTO newStudent)
        {
            var response = await _studentService.AddBatchAdmin(_currentUserService.ToUserScope(), newStudent, _currentUserService.UserId);
            return response.ToActionResult();
        }


        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpPut("batch_admin", Name = "UpdateBatchAdmin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<StudentDTO>>> UpdateBatchAdmin([FromQuery]IdParameter idParameter,[FromBody] UpdateStudentDTO updateStudent)
        {
            var response = await _studentService.UpdateBatchAdmin(idParameter.Id,_currentUserService.ToUserScope(), updateStudent, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin,BatchAdmin")]
        [HttpPost("student", Name = "AddStudent")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<StudentDTO>>> AddStudent([FromBody] AddStudentDTO newStudent)
        {
            var response = await _studentService.AddStudent(_currentUserService.ToUserScope(), newStudent, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin,BatchAdmin")]
        [HttpPut("student", Name = "UpdateStudent")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<StudentDTO>>> UpdateStudent([FromQuery] IdParameter idParameter, [FromBody] UpdateStudentDTO updateStudent)
        {
            var response = await _studentService.UpdateStudent(idParameter.Id, _currentUserService.ToUserScope(), updateStudent, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin,BatchAdmin")]
        [HttpPut("status", Name = "UpdateStudentStatus")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<StudentDTO>>> UpdateStudentStatus
            ([FromQuery]StudentIdParameter studentParameter, [FromQuery]StudentStatusIdParameter statusParameter)
        {
            var response = await _studentService.UpdateStudentStatus
                (_currentUserService.ToUserScope(), studentParameter.StudentId, statusParameter.StudentStatusId, _currentUserService.UserId);

            return response.ToActionResult();
        }
    }
}
