using Contracts.Common.Messages;
using Contracts.Requests.AcademicRequests.CollegeRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses;
using Contracts.Responses.CollegeResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.CollegeInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Extensions;

namespace UniNet.Controllers.AcademicControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollegeController : ControllerBase
    {
        private readonly ICollegeService _collegeService;
        private readonly ICurrentUserService _currentUserService;
        public CollegeController(ICollegeService collegeService, ICurrentUserService currentUserService)
        {
            _collegeService = collegeService;
            _currentUserService = currentUserService;
        }

        [Authorize]
        [HttpGet(Name ="GetAllColleges")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<CollegeDTO>>> GetAllColleges([FromQuery]PagedResultParameters @parameters)
        {
            var colleges = await _collegeService.GetColleges(parameters.PageNumber, parameters.PageSize);
            return  colleges.ToPagedActioneResult();
        }

        [Authorize]
        [HttpGet("by-universityId",Name = "GetCollegesPerUniversity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<CollegeDTO>>> GetCollegesPerUniversity([FromQuery]IdParameter @idParameter, [FromQuery]PagedResultParameters @parameters)
        {
            var colleges = await _collegeService.GetCollegesPerUniversity(idParameter.Id,parameters.PageNumber,parameters.PageSize);
            return colleges.ToPagedActioneResult();
        }


        [Authorize]
        [HttpGet("by-id",Name ="GetCollegeById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CollegeDTO>> GetCollegeById([FromQuery]IdParameter @idParameter)
        {
            var college = await _collegeService.GetCollegeDTOById(idParameter.Id);
            return college.GetResourceEndpoints(idParameter.Id, typeof(College).Name);
        }

        [Authorize]
        [HttpGet("by-name", Name = "GetCollegeByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CollegeDTO>> GetCollegeByName([FromQuery]IdParameter @universityId,[FromQuery] BaseStringParametre @Parameter)
        {
            var college = await _collegeService.GetCollegeDTOByName(universityId.Id,Parameter.Name);
            return college.GetResourceEndpoints(Parameter.Name, typeof(College).Name);
        }

        [Authorize]
        [HttpDelete(Name ="DeleteCollege")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Delete([FromQuery]IdParameter @idParameter)
        {
            var result = await _collegeService.Delete(idParameter.Id);
            if(!result)
                return NotFound(DeleteMessage.DeletionFailed<College>(idParameter.Id));

            return Ok(DeleteMessage.DeletedSuccessfuly<College>(idParameter.Id));
        }

        [Authorize]
        [HttpPost(Name ="AddCollege")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AddUpdateServiceResponse<CollegeDTO>>> AddCollege([FromBody]AddCollegeDTO newCollege)
        {
            var response = await _collegeService.AddCollege(newCollege, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize]
        [HttpPut(Name = "UpdateCollege")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AddUpdateServiceResponse<CollegeDTO>>> UpdateCollege([FromQuery]IdParameter @parameter,[FromBody] UpdateCollegeDTO updatedCollege)
        {
            var response = await _collegeService.UpdateCollege(parameter.Id,updatedCollege, _currentUserService.UserId);
            return response.ToActionResult();
        }

    }
}
