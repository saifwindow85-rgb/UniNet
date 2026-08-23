using Contracts.Common.Messages;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.SubjectRequests;
using Contracts.Responses;
using Contracts.Responses.StudyResponses.SubjectResponses;
using Contracts.Results;
using Domain.Entities.Study;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Domain.Interfaces.StudyInterfaces.SubjectInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Extensions;

namespace UniNet.Controllers.StudyControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthorizationService _authorizationService;
        public SubjectController(ISubjectService subjectService, ICurrentUserService currentUserService, IAuthorizationService authorizationService)
        {
            _subjectService = subjectService;
            _currentUserService = currentUserService;
            _authorizationService = authorizationService;
        }

        [Authorize(Roles = "Super Admin")]
        [HttpGet(Name = "GetAllSubjects")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<SubjectDTO>>> GetAllSubjects([FromQuery] SubjectFilterDTO? filter, [FromQuery] PagedResultParameters @pagedResultParameters)
        {
            var subjects = await _subjectService.GetAll(filter, pagedResultParameters.PageNumber, pagedResultParameters.PageSize);
            return subjects.ToPagedActioneResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpGet("by-departmentId", Name = "GetSubjectsPerDepartment")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<SubjectDTO>>> GetSubjectsPerDepartment([FromQuery] SubjectFilterDTO? filter, [FromQuery] PagedResultParameters @pagedResultParameters)
        {
            var subjects = await _subjectService.GetSubjectsPerDepartments(_currentUserService.ToUserScope(), filter, pagedResultParameters.PageNumber, pagedResultParameters.PageSize);
            return subjects.ToPagedActioneResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpGet("by-id", Name = "GetSubjectById")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetaieldSubjectDTO>> GetSubjectById([FromQuery] SubjectIdParameter @subjectIdParameter)
        {
            var subjectInfo = await _subjectService.GetSubjectAuthorizationInfoAsync(subjectIdParameter.SubjectId);
            if (subjectInfo == null)
                return NotFound(ErrorMessages.NotFound<Subject>(subjectIdParameter.SubjectId));

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, subjectInfo, "SubjectOwnerPolicy");
            if (!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var subject = await _subjectService.GetDetaieldSubjectDTOById(subjectIdParameter.SubjectId);
            return subject.GetResourceEndpoints(subjectIdParameter.SubjectId, typeof(Subject).Name);
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpPost(Name = "AddSubject")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<SubjectDTO>>> AddSubject([FromBody] AddSubjectDTO newSubject)
        {
            var response = await _subjectService.AddSubject(_currentUserService.ToUserScope(), newSubject, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpPut(Name = "UpdateSubject")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<SubjectDTO>>> UpdateSubject([FromQuery] SubjectIdParameter @subjectIdParameter, [FromBody] UpdateSubjectDTO updatedSubject)
        {
            var response = await _subjectService.UpdateSubject(_currentUserService.ToUserScope(), updatedSubject, subjectIdParameter.SubjectId, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpDelete(Name = "DeleteSubject")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> DeleteSubject([FromQuery] SubjectIdParameter @subjectIdParameter)
        {
            var subjectInfo = await _subjectService.GetSubjectAuthorizationInfoAsync(subjectIdParameter.SubjectId);
            if (subjectInfo == null)
                return NotFound(ErrorMessages.NotFound<Subject>(subjectIdParameter.SubjectId));

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, subjectInfo, "SubjectOwnerPolicy");
            if (!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var result = await _subjectService.Delete(subjectIdParameter.SubjectId);
            return result.ToDeleteActionResult<Subject>(subjectIdParameter.SubjectId);
        }
    }
}
