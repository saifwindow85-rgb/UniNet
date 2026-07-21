using Contracts.Common.Messages;
using Contracts.Requests.AcademicRequests.DepartmentRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses;
using Contracts.Responses.AcademicResponses.DepartmentResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.DepartmentInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Extensions;

namespace UniNet.Controllers.AcademicControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly ICurrentUserService _currentUserService;
        public DepartmentController(IDepartmentService departmentService, ICurrentUserService currentUserService)
        {
            _departmentService = departmentService;
            _currentUserService = currentUserService;
        }

        [Authorize]
        [HttpGet(Name ="GetAllDepartments")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<DepartmentDTO>>> GetAllDepartments([FromQuery]PagedResultParameters @pagedResultParameters)
        {
            var departments = await _departmentService.GetAllDepartments(@pagedResultParameters.PageNumber, @pagedResultParameters.PageSize);
            return departments.ToPagedActioneResult();
        }

        [Authorize]
        [HttpGet("collegeId",Name = "GetDepartmentsPerCollege")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<DepartmentDTO>>> GetDepartmentsPerCollege
            ([FromQuery]CollegeIdParameter @collegeParameter, [FromQuery]PagedResultParameters @pagedResultParameters)
        {
            var departments = await _departmentService.GetDepartmentsPerCollege(collegeParameter.CollegeId,pagedResultParameters.PageNumber,@pagedResultParameters.PageSize);
            return departments.ToPagedActioneResult();
        }


        [Authorize]
        [HttpGet("by-id",Name ="GetDepartmentById")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DepartmentDTO>> GetDepartmentById([FromQuery]DepartmentIdParameter @departmentParameter)
        {
            var department = await _departmentService.GetDTOById(departmentParameter.DepartmentId);
            return department.GetResourceEndpoints(departmentParameter.DepartmentId, typeof(Department).Name);
        }


        [Authorize]
        [HttpPost( Name = "AddDepartment")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<DepartmentDTO>>> AddDepartment([FromBody]AddDepartmentDTO newDepartment)
        {
            var response = await _departmentService.AddDepartment(newDepartment, _currentUserService.UserId);
            return response.ToActionResult();
        }
        [Authorize]
        [HttpPut(Name = "UpdateDepartment")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<AddUpdateServiceResponse<DepartmentDTO>>> UpdateDepartment
            ([FromQuery]DepartmentIdParameter @departmentIdParameter,[FromBody] UpdateDepartmentDTO UpdatedDepartment)
        {
            var response = await _departmentService.UpdateDepartment(departmentIdParameter.DepartmentId,UpdatedDepartment, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize]
        [HttpDelete(Name = "Delete")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete([FromQuery]DepartmentIdParameter @departmentIdParameter)
        {
            var result = await _departmentService.Delete(departmentIdParameter.DepartmentId);
            if (!result)
                return NotFound(DeleteMessage.DeletionFailed<Department>(departmentIdParameter.DepartmentId));

            return Ok(DeleteMessage.DeletedSuccessfuly<Department>(departmentIdParameter.DepartmentId));
        }
    }
}
