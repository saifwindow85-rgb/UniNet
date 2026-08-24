using Contracts.Common.Messages;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.SemesterRequests;
using Contracts.Responses;
using Contracts.Responses.StudyResponses.SemesterResponses;
using Contracts.Results;
using Domain.Entities.Study;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Domain.Interfaces.StudyInterfaces.SemesterInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Extensions;

namespace UniNet.Controllers.StudyControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SemesterController : ControllerBase
    {
        private readonly ISemesterService _semesterService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthorizationService _authorizationService;
        public SemesterController(ISemesterService semesterService, ICurrentUserService currentUserService, IAuthorizationService authorizationService)
        {
            _semesterService = semesterService;
            _currentUserService = currentUserService;
            _authorizationService = authorizationService;
        }

        [Authorize(Roles = "Super Admin")]
        [HttpGet(Name = "GetAllSemesters")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<SemesterDTO>>> GetAllSemesters([FromQuery] SemesterFilterDTO? filter, [FromQuery] PagedResultParameters @pagedResultParameters)
        {
            var semesters = await _semesterService.GetAll(filter, pagedResultParameters.PageNumber, pagedResultParameters.PageSize);
            return semesters.ToPagedActioneResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin")]
        [HttpGet("by-universityId", Name = "GetSemestersPerUniversity")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<SemesterDTO>>> GetSemestersPerUniversity([FromQuery] SemesterFilterDTO? filter, [FromQuery] PagedResultParameters @pagedResultParameters)
        {
            var semesters = await _semesterService.GetSemestersPerUniversity(_currentUserService.ToUserScope(), filter, pagedResultParameters.PageNumber, pagedResultParameters.PageSize);
            return semesters.ToPagedActioneResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin")]
        [HttpGet("by-id", Name = "GetSemesterById")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetaieldSemesterDTO>> GetSemesterById([FromQuery] SemesterIdParameter @semesterIdParameter)
        {
            var semesterInfo = await _semesterService.GetSemesterAuthorizationInfoAsync(semesterIdParameter.SemesterId);
            if (semesterInfo == null)
                return NotFound(ErrorMessages.NotFound<Semester>(semesterIdParameter.SemesterId));

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, semesterInfo, "SemesterOwnerPolicy");
            if (!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var semester = await _semesterService.GetDetaieldSemesterDTOById(semesterIdParameter.SemesterId);
            return semester.GetResourceEndpoints(semesterIdParameter.SemesterId, typeof(Semester).Name);
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin")]
        [HttpGet("current", Name = "GetCurrentSemester")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SemesterDTO>> GetCurrentSemester([FromQuery] UniversityIdParameter @universityIdParameter)
        {
            var authorizationInfo = new Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos.SemesterAuthorizationInfo { UniversityId = universityIdParameter.UniversityId };
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, authorizationInfo, "SemesterOwnerPolicy");
            if (!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var semester = await _semesterService.GetCurrentSemester(universityIdParameter.UniversityId);
            return semester.GetResourceEndpoints(universityIdParameter.UniversityId, typeof(Semester).Name);
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin")]
        [HttpPost(Name = "AddSemester")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<SemesterDTO>>> AddSemester([FromBody] AddSemesterDTO newSemester)
        {
            var response = await _semesterService.AddSemester(_currentUserService.ToUserScope(), newSemester, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin")]
        [HttpPut(Name = "UpdateSemester")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<SemesterDTO>>> UpdateSemester([FromQuery] SemesterIdParameter @semesterIdParameter, [FromBody] UpdateSemesterDTO updatedSemester)
        {
            var response = await _semesterService.UpdateSemester(_currentUserService.ToUserScope(), updatedSemester, semesterIdParameter.SemesterId, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin")]
        [HttpPut("end", Name = "EndSemester")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<SemesterDTO>>> EndSemester([FromQuery] SemesterIdParameter @semesterIdParameter)
        {
            var response = await _semesterService.EndSemester(_currentUserService.ToUserScope(), semesterIdParameter.SemesterId, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin")]
        [HttpDelete(Name = "DeleteSemester")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> DeleteSemester([FromQuery] SemesterIdParameter @semesterIdParameter)
        {
            var semesterInfo = await _semesterService.GetSemesterAuthorizationInfoAsync(semesterIdParameter.SemesterId);
            if (semesterInfo == null)
                return NotFound(ErrorMessages.NotFound<Semester>(semesterIdParameter.SemesterId));

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, semesterInfo, "SemesterOwnerPolicy");
            if (!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var result = await _semesterService.Delete(semesterIdParameter.SemesterId);
            return result.ToDeleteActionResult<Semester>(semesterIdParameter.SemesterId);
        }
    }
}
