using Contracts.Common.Messages;
using Contracts.Requests.AcademicRequests.CollegeRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses;
using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.CollegeInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Extensions;
using Contracts.Responses.AcademicResponses.CollegeResponses;

namespace UniNet.Controllers.AcademicControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollegeController : ControllerBase
    {
        private readonly ICollegeService _collegeService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthorizationService _authorizationService;
        public CollegeController(ICollegeService collegeService, ICurrentUserService currentUserService, IAuthorizationService authorizationService)
        {
            _collegeService = collegeService;
            _currentUserService = currentUserService;
            _authorizationService = authorizationService;
        }

        [Authorize(Roles = "Super Admin")]
        [HttpGet(Name ="GetAllColleges")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<CollegeDTO>>> GetAllColleges([FromQuery]CollegeFilter?filter,[FromQuery]PagedResultParameters @parameters)
        {
            var colleges = await _collegeService.GetColleges(filter,parameters.PageNumber, parameters.PageSize);
            return  colleges.ToPagedActioneResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin")]
        [HttpGet("by-universityId",Name = "GetCollegesPerUniversity")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<CollegeDTO>>> GetCollegesPerUniversity
            ([FromQuery]CollegeFilter?filter, [FromQuery]PagedResultParameters @parameters)
        {
            var colleges = await _collegeService.GetCollegesPerUniversity(_currentUserService.ToUserScope(),filter,parameters.PageNumber,parameters.PageSize);
            return colleges.ToPagedActioneResult();
        }


        [Authorize(Roles = "Super Admin,UniversityAdmin")]
        [HttpGet("by-id",Name ="GetCollegeById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CollegeDTO>> GetCollegeById([FromQuery]CollegeIdParameter @collegeIdParameter)
        {
            var collegeAthorizationInfo = await _collegeService.GetCollegeAuthorizationInfo(collegeIdParameter.CollegeId);
            if (collegeAthorizationInfo == null)
                return NotFound(ErrorMessages.NotFound<College>(collegeIdParameter.CollegeId));

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, collegeAthorizationInfo, "CollegeOwnerPolicy");
            if (!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var college = await _collegeService.GetCollegeDTOById(@collegeIdParameter.CollegeId);
            return college.GetResourceEndpoints(@collegeIdParameter.CollegeId, typeof(College).Name);
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin")]
        [HttpGet("by-name", Name = "GetCollegeByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
    

        [Authorize(Roles = "Super Admin,UniversityAdmin")]
        [HttpDelete(Name ="DeleteCollege")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Delete([FromQuery]CollegeIdParameter @collegeIdParameter)
        {
            var collegeAuthorizationInfo = await _collegeService.GetCollegeAuthorizationInfo(collegeIdParameter.CollegeId);
            if (collegeAuthorizationInfo == null)
                return NotFound(ErrorMessages.NotFound<College>(collegeIdParameter.CollegeId));
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, collegeAuthorizationInfo, "CollegeOwnerPolicy");
            if(!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var result = await _collegeService.Delete(collegeIdParameter.CollegeId);
            return result.ToDeleteActionResult<College>(collegeIdParameter.CollegeId);
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin")]
        [HttpPost(Name ="AddCollege")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AddUpdateServiceResponse<CollegeDTO>>> AddCollege([FromBody]AddCollegeDTO newCollege)
        {
            var response = await _collegeService.AddCollege(_currentUserService.ToUserScope(),newCollege, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin")]
        [HttpPut(Name = "UpdateCollege")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AddUpdateServiceResponse<CollegeDTO>>> UpdateCollege([FromQuery]CollegeIdParameter @collegeIdParameter,[FromBody] UpdateCollegeDTO updatedCollege)
        {
            var response = await _collegeService.UpdateCollege(collegeIdParameter.CollegeId, updatedCollege, _currentUserService.UserId);
            return response.ToActionResult();
        }

    }
}
