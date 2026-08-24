using Contracts.Common.Messages;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.SectionSubjectRequests;
using Contracts.Responses;
using Contracts.Responses.StudyResponses.SectionSubjectResponses;
using Contracts.Results;
using Domain.Entities.Study;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Domain.Interfaces.StudyInterfaces.SectionSubjectInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Extensions;

namespace UniNet.Controllers.StudyControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionSubjectController : ControllerBase
    {
        private readonly ISectionSubjectService _sectionSubjectService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthorizationService _authorizationService;
        public SectionSubjectController(ISectionSubjectService sectionSubjectService, ICurrentUserService currentUserService, IAuthorizationService authorizationService)
        {
            _sectionSubjectService = sectionSubjectService;
            _currentUserService = currentUserService;
            _authorizationService = authorizationService;
        }

        [Authorize(Roles = "Super Admin")]
        [HttpGet(Name = "GetAllSectionSubjects")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<SectionSubjectDTO>>> GetAllSectionSubjects([FromQuery] SectionSubjectFilterDTO? filter, [FromQuery] PagedResultParameters @pagedResultParameters)
        {
            var sectionSubjects = await _sectionSubjectService.GetAll(filter, pagedResultParameters.PageNumber, pagedResultParameters.PageSize);
            return sectionSubjects.ToPagedActioneResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin,BatchAdmin")]
        [HttpGet("scoped", Name = "GetSectionSubjectsPerScope")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<SectionSubjectDTO>>> GetSectionSubjectsPerScope([FromQuery] SectionSubjectFilterDTO? filter, [FromQuery] PagedResultParameters @pagedResultParameters)
        {
            var sectionSubjects = await _sectionSubjectService.GetSectionSubjectsPerScope(_currentUserService.ToUserScope(), filter, pagedResultParameters.PageNumber, pagedResultParameters.PageSize);
            return sectionSubjects.ToPagedActioneResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin,BatchAdmin")]
        [HttpGet("by-id", Name = "GetSectionSubjectById")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetaieldSectionSubjectDTO>> GetSectionSubjectById([FromQuery] SectionSubjectIdParameter @sectionSubjectIdParameter)
        {
            var sectionSubjectInfo = await _sectionSubjectService.GetSectionSubjectAuthorizationInfoAsync(sectionSubjectIdParameter.SectionSubjectId);
            if (sectionSubjectInfo == null)
                return NotFound(ErrorMessages.NotFound<SectionSubject>(sectionSubjectIdParameter.SectionSubjectId));

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, sectionSubjectInfo, "SectionSubjectOwnerPolicy");
            if (!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var sectionSubject = await _sectionSubjectService.GetDetaieldSectionSubjectDTOById(sectionSubjectIdParameter.SectionSubjectId);
            return sectionSubject.GetResourceEndpoints(sectionSubjectIdParameter.SectionSubjectId, typeof(SectionSubject).Name);
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin,BatchAdmin")]
        [HttpPost(Name = "AddSectionSubject")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AddUpdateServiceResponse<SectionSubjectDTO>>> AddSectionSubject([FromBody] AddSectionSubjectDTO newSectionSubject)
        {
            var response = await _sectionSubjectService.AddSectionSubject(_currentUserService.ToUserScope(), newSectionSubject, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin,BatchAdmin")]
        [HttpPut(Name = "UpdateSectionSubject")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<SectionSubjectDTO>>> UpdateSectionSubject([FromQuery] SectionSubjectIdParameter @sectionSubjectIdParameter, [FromBody] UpdateSectionSubjectDTO updatedSectionSubject)
        {
            var response = await _sectionSubjectService.UpdateSectionSubject(_currentUserService.ToUserScope(), updatedSectionSubject, sectionSubjectIdParameter.SectionSubjectId, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin,BatchAdmin")]
        [HttpDelete(Name = "DeleteSectionSubject")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> DeleteSectionSubject([FromQuery] SectionSubjectIdParameter @sectionSubjectIdParameter)
        {
            var sectionSubjectInfo = await _sectionSubjectService.GetSectionSubjectAuthorizationInfoAsync(sectionSubjectIdParameter.SectionSubjectId);
            if (sectionSubjectInfo == null)
                return NotFound(ErrorMessages.NotFound<SectionSubject>(sectionSubjectIdParameter.SectionSubjectId));

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, sectionSubjectInfo, "SectionSubjectOwnerPolicy");
            if (!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var result = await _sectionSubjectService.Delete(sectionSubjectIdParameter.SectionSubjectId);
            return result.ToDeleteActionResult<SectionSubject>(sectionSubjectIdParameter.SectionSubjectId);
        }
    }
}
