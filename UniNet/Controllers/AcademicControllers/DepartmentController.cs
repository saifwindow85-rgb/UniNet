using Contracts.Common.Extensions;
using Contracts.Common.Messages;
using Contracts.Requests.AcademicRequests.CommonAcademicRequests;
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
        private readonly IAuthorizationService _authorizationService;
        public DepartmentController(IDepartmentService departmentService, ICurrentUserService currentUserService, IAuthorizationService authorizationService)
        {
            _departmentService = departmentService;
            _currentUserService = currentUserService;
            _authorizationService = authorizationService;
        }

        [Authorize(Roles = "Super Admin")]
        [HttpGet(Name ="GetAllDepartments")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<DepartmentDTO>>> GetAllDepartments([FromQuery]PagedResultParameters @pagedResultParameters,AcademicFilter?filter)
        {
            var departments = await _departmentService.GetAllDepartments(filter,@pagedResultParameters.PageNumber, @pagedResultParameters.PageSize);
            return departments.ToPagedActioneResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin")]
        [HttpGet("collegeId",Name = "GetDepartmentsPerCollege")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<DepartmentDTO>>> GetDepartmentsPerCollege
            ([FromQuery]CollegeIdParameter @collegeParameter, [FromQuery]AcademicFilter?filter, [FromQuery]PagedResultParameters @pagedResultParameters)
        {
            var departments = await _departmentService.GetDepartmentsPerCollege(_currentUserService.ToUserScope(),filter,pagedResultParameters.PageNumber,@pagedResultParameters.PageSize);
            return departments.ToPagedActioneResult();
        }


        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpGet("by-id",Name ="GetDepartmentById")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DepartmentDTO>> GetDepartmentById([FromQuery]DepartmentIdParameter @departmentParameter)
        {
            var departmentAuthorizationInfo = await _departmentService.GetDepartmentAuthorizationInfoAsync(departmentParameter.DepartmentId);
            if(departmentAuthorizationInfo == null)
                return NotFound(ErrorMessages.NotFound<Department>(departmentParameter.DepartmentId));

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, departmentAuthorizationInfo, "DepartmentOwnerPolicy");
            if (!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var department = await _departmentService.GetDTOById(departmentParameter.DepartmentId);
            return department.GetResourceEndpoints(departmentParameter.DepartmentId, typeof(Department).Name);
        }


        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin")]
        [HttpPost( Name = "AddDepartment")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<DepartmentDTO>>> AddDepartment([FromBody]AddDepartmentDTO newDepartment)
        {
            var response = await _departmentService.AddDepartment(_currentUserService.ToUserScope(),newDepartment, _currentUserService.UserId);
            return response.ToActionResult();
        }
        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin")]
        [HttpPut(Name = "UpdateDepartment")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<AddUpdateServiceResponse<DepartmentDTO>>> UpdateDepartment
            ([FromQuery]DepartmentIdParameter @departmentIdParameter,[FromBody] UpdateDepartmentDTO UpdatedDepartment)
        {
            var response = await _departmentService.UpdateDepartment(_currentUserService.ToUserScope(),departmentIdParameter.DepartmentId,UpdatedDepartment, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin")]
        [HttpDelete(Name = "Delete")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete([FromQuery]DepartmentIdParameter @departmentIdParameter)
        {
            var departmentAuthorizationInfo = await _departmentService.GetDepartmentAuthorizationInfoAsync(departmentIdParameter.DepartmentId);
            if(departmentAuthorizationInfo == null)
            {
                return NotFound(ErrorMessages.NotFound<Department>(departmentIdParameter.DepartmentId));
            }
             
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, departmentAuthorizationInfo, "DepartmentOwnerPolicy");
            if (!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var result = await _departmentService.Delete(departmentIdParameter.DepartmentId);
            return result.ToDeleteActionResult<Department>(departmentIdParameter.DepartmentId);
        }
    }
}
