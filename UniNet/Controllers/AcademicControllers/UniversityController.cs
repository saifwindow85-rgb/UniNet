using Contracts.Common.Messages;
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
        public UniversityController(IUniversityService universityService, ICurrentUserService currentUserService)
        {
            _uinversityService = universityService;
            _currentUserService = currentUserService;
        }


        [Authorize]
        [HttpGet(Name ="GetAllUniversities")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<UniversityDTO>>> GetAllUniversities([FromQuery]PagedResultParameters @parameters)
        {
            var universities = await _uinversityService.GetAllUniversities(parameters.PageNumber, parameters.PageSize);
            return universities.ToPagedActioneResult();
        }

        [Authorize]
        [HttpGet("by-id",Name ="GetUniversityById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UniversityDTO>> GetUniversityById([FromQuery]IdParameter @parameter)
        {
            var university = await _uinversityService.FindUniversityDTOById(@parameter.Id);
            if(university == null)
                return NotFound(ErrorMessages.NotFound<University>(parameter.Id));

            return Ok(university);
        }


        [Authorize]
        [HttpDelete(Name ="DeleteUniversity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete([FromQuery] IdParameter @parameter)
        {
            var result = await _uinversityService.Delete(parameter.Id);
            if (!result)
                return NotFound(DeleteMessage.DeletionFailed<University>(parameter.Id));
            return Ok(DeleteMessage.DeletedSuccessfuly<University>(parameter.Id));
        }

        [Authorize]
        [HttpPost(Name ="AddUniversity")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<UniversityDTO>>> AddUniversity([FromBody]AddUniversityDTO newUniversity)
        {
            var response = await _uinversityService.AddUniversity(newUniversity, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize]
        [HttpPut(Name ="UpdateUniversity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<UniversityDTO>>> UpdateUniversity([FromQuery]IdParameter @parameter,
            [FromBody]UpdateUniversityDTO updatedUniversity)
        {
            var response = await _uinversityService.UpdateUniversity(parameter.Id, updatedUniversity, _currentUserService.UserId);
            return response.ToActionResult();
        }
    }
}
