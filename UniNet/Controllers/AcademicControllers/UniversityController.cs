using Contracts.Common.Messages;
using Contracts.Requests.AcademicRequests.CommonAcademicRequests;
using Contracts.Requests.AcademicRequests.UniversityRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses;
using Contracts.Responses.AcademicResponses.UniversityResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.UniversityInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Extensions;

namespace UniNet.Controllers.AcademicControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversityController : ControllerBase
    {
        private readonly IUniversityService _uinversityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthorizationService _authorizationService;
        public UniversityController(IUniversityService universityService, ICurrentUserService currentUserService, IAuthorizationService authorizationService)
        {
            _uinversityService = universityService;
            _currentUserService = currentUserService;   
            _authorizationService = authorizationService;
        }


        [Authorize(Roles ="Super Admin")]
        [HttpGet(Name ="GetAllUniversities")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<UniversityDTO>>> GetAllUniversities([FromQuery]AcademicFilter?filter,[FromQuery]PagedResultParameters @parameters)
        {
            var universities = await _uinversityService.GetAllUniversities(filter,parameters.PageNumber, parameters.PageSize);
            return universities.ToPagedActioneResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin")]
        [HttpGet("by-id",Name ="GetUniversityById")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UniversityDTO>> GetUniversityById([FromQuery]UniversityIdParameter @universityIdParameter)
        {
            var universityAuthorizationInfo = await _uinversityService.GetUniversityAuthorizationInfoAsync(universityIdParameter.UniversityId);
            if (universityAuthorizationInfo == null)
                return NotFound(ErrorMessages.NotFound<University>(universityIdParameter.UniversityId));

            var authorization = await _authorizationService.AuthorizeAsync(User, universityAuthorizationInfo, "UniversityOwnerPolicy");
            if (!authorization.Succeeded)
                return authorization.NotAuthorized();

            var university = await _uinversityService.FindUniversityDTOById(@universityIdParameter.UniversityId);
            return university.GetResourceEndpoints(universityIdParameter.UniversityId, typeof(University).Name);
        }


        [Authorize(Roles = "Super Admin")]
        [HttpDelete(Name ="DeleteUniversity")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Delete([FromQuery] UniversityIdParameter @universityIdParameter)
        {
            var result = await _uinversityService.Delete(universityIdParameter.UniversityId);
            return result.ToDeleteActionResult<University>(universityIdParameter.UniversityId);
        }

        [Authorize(Roles = "Super Admin")]
        [HttpPost(Name ="AddUniversity")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AddUpdateServiceResponse<UniversityDTO>>> AddUniversity([FromBody]AddUniversityDTO newUniversity)
        {
            var response = await _uinversityService.AddUniversity(newUniversity, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin")]
        [HttpPut(Name ="UpdateUniversity")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AddUpdateServiceResponse<UniversityDTO>>> UpdateUniversity([FromQuery]UniversityIdParameter @universityIdParameter,
            [FromBody]UpdateUniversityDTO updatedUniversity)
        {
            var response = await _uinversityService.UpdateUniversity(universityIdParameter.UniversityId, updatedUniversity, _currentUserService.UserId);
            return response.ToActionResult();
        }
    }
}
