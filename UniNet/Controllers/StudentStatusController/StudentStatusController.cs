using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudentRequests;
using Contracts.Responses;
using Contracts.Responses.StudentResponses;
using Contracts.Results;
using Domain.Entities.Students;
using Domain.Interfaces.StudentStatusInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Extensions;

namespace UniNet.Controllers.StudentStatusController
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentStatusController : ControllerBase
    {
        private readonly IStatusService _statusService;
        public StudentStatusController(IStatusService statusService)
        {
            _statusService = statusService;
        }

        [Authorize(Roles ="Super Admin")]
        [HttpGet(Name ="GetStatuses")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<StudentStatusDTO>>> GetStatuses([FromQuery]PagedResultParameters pagedResultParameters)
        {
            var statuses = await _statusService.GetStatuses(pagedResultParameters.PageNumber, pagedResultParameters.PageSize);
            return statuses.ToPagedActioneResult();
        }

        [Authorize(Roles = "Super Admin")]
        [HttpGet("by_id",Name = "GetStatusById")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<StudentStatusDTO>> GetStatusById([FromQuery]IdParameter idParameter)
        {
            var status = await _statusService.GetDTOById(idParameter.Id);
            return status.GetResourceEndpoints(idParameter.Id, typeof(StudentStatus).Name);
        }

        [Authorize(Roles = "Super Admin")]
        [HttpGet("by_name", Name = "GetStatusByName")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<StudentStatusDTO>> GetStatusByName([FromQuery]BaseStringParametre stringParametre)
        {
            var status = await _statusService.GetDTOByName(stringParametre.Name);
            return status.GetResourceEndpoints(stringParametre.Name,typeof(StudentStatus).Name);
        }

        [Authorize(Roles ="Super Admin")]
        [HttpPost(Name ="AddStatus")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<AddUpdateServiceResponse<StudentStatusDTO>>> AddStatus([FromBody]AddUpdateStudentStatusDTO newStatus)
        {
            var response = await _statusService.AddStatus(newStatus);
            return response.ToActionResult();
        }

        [Authorize(Roles ="Super Admin")]
        [HttpPut(Name ="UpdateStatus")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AddUpdateServiceResponse<StudentStatusDTO>>> UpdateStatus
            ([FromQuery]IdParameter idParameter, [FromBody]AddUpdateStudentStatusDTO updatedStatus)
        {
            var response = await _statusService.UpdateStatus(idParameter.Id, updatedStatus);
            return response.ToActionResult();
        }
    }
}
