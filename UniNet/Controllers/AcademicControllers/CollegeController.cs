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

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpGet(Name ="GetAllColleges")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<CollegeDTO>>> GetAllColleges([FromQuery]PagedResultParameters @parameters)
        {
            var colleges = await _collegeService.GetColleges(parameters.PageNumber, parameters.PageSize);
            return  colleges.ToPagedActioneResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpGet("by-universityId",Name = "GetCollegesPerUniversity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<CollegeDTO>>> GetCollegesPerUniversity([FromQuery]UniversityIdParameter universityIdParameter, [FromQuery]PagedResultParameters @parameters)
        {
            var colleges = await _collegeService.GetCollegesPerUniversity(universityIdParameter.UniversityId,parameters.PageNumber,parameters.PageSize);
            return colleges.ToPagedActioneResult();
        }


        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpGet("by-id",Name ="GetCollegeById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CollegeDTO>> GetCollegeById([FromQuery]CollegeIdParameter @collegeIdParameter)
        {
            var college = await _collegeService.GetCollegeDTOById(@collegeIdParameter.CollegeId);
            return college.GetResourceEndpoints(@collegeIdParameter.CollegeId, typeof(College).Name);
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpGet("by-name", Name = "GetCollegeByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CollegeDTO>> GetCollegeByName([FromQuery]UniversityIdParameter @universityIdParameter,[FromQuery] BaseStringParametre @Parameter)
        {
            var college = await _collegeService.GetCollegeDTOByName(@universityIdParameter.UniversityId, Parameter.Name);
            return college.GetResourceEndpoints(Parameter.Name, typeof(College).Name);
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpDelete(Name ="DeleteCollege")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Delete([FromQuery]CollegeIdParameter @collegeIdParameter)
        {
            var result = await _collegeService.Delete(collegeIdParameter.CollegeId);
            return result.ToDeleteActionResult<College>(collegeIdParameter.CollegeId);
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpPost(Name ="AddCollege")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AddUpdateServiceResponse<CollegeDTO>>> AddCollege([FromBody]AddCollegeDTO newCollege)
        {
            var response = await _collegeService.AddCollege(newCollege, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpPut(Name = "UpdateCollege")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
