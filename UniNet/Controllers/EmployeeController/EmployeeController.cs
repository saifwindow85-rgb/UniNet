using Contracts.Requests.EmployeeRequests;
using Contracts.Requests.EmployeeRequests.CollegeAdminRequests;
using Contracts.Requests.EmployeeRequests.DepartmentAdminRequests;
using Contracts.Requests.EmployeeRequests.UniversityAdminRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses;
using Contracts.Responses.EmployeeResponse;
using Contracts.Results;
using Domain.Interfaces.EmployeeInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Extensions;

namespace UniNet.Controllers.EmployeeController
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmployeeService _employeeService;
        public EmployeeController(ICurrentUserService currentUserService, IEmployeeService employeeService)
        {
            _currentUserService = currentUserService;
            _employeeService = employeeService;
        }

        [Authorize(Roles ="Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpGet("GetEmployees")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<EmployeeDTO>>>GetEmployees
            ([FromQuery]EmployeeFilter? filter, [FromQuery]PagedResultParameters pagedResultParameters)
        {
            var scope = _currentUserService.ToEmployeeScope();
            var employees = await _employeeService.GetEmployees(filter,scope,pagedResultParameters.PageNumber, pagedResultParameters.PageSize);
            return employees.ToPagedActioneResult();
        }

        [Authorize(Roles = "Super Admin")]
        [HttpPost("university_admin",Name ="AdduniversityAdmin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<EmployeeDTO>>> AddUniversityAdmin([FromBody]AddUniversityAdminDTO newUniversityAdmin)
        {
            var response = await _employeeService.AddUniversityAdmin(newUniversityAdmin, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin")]
        [HttpPut("university_admin", Name = "UpdateuniversityAdmin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<EmployeeDTO>>>
            UpdateEmployee([FromQuery]EmployeeIdParameter employeeIdParameter,[FromBody]UpdateUniversityAdminDTO updatedUnivrsityAdmin)
        {
            var response = await _employeeService.UpdateUniversityAdmin(employeeIdParameter.EmployeeId, updatedUnivrsityAdmin, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin")]
        [HttpPost("college-admin", Name = "AddCollegeAdmin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<EmployeeDTO>>> AddCollegeAdmin([FromBody] AddCollegeAdminDTO newCollegeAdmin)
        {
            var scope = _currentUserService.ToEmployeeScope();
            var response = await _employeeService.AddCollegeAdmin(scope,newCollegeAdmin, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin")]
        [HttpPost("department-admin", Name = "AddDepartmentAdmin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<EmployeeDTO>>> AddDepartmentDTO([FromBody]AddDepartmentAdminDTO newDepartmentDTO)
        {
            var scope = _currentUserService.ToEmployeeScope();
            var response = await _employeeService.AddDepartmentAdmin(scope,newDepartmentDTO, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin")]
        [HttpPut("college_admin", Name = "UpdateCollegeAdmin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<EmployeeDTO>>> UpdateCollegeAdmin([FromQuery]EmployeeIdParameter employeeIdParameter, [FromBody]UpdateCollegeAdminDTO updatedCollegeAdmin)
        {
            var scope = _currentUserService.ToEmployeeScope();
            var response = await _employeeService.UpdateCollegeAdmin(scope, employeeIdParameter.EmployeeId, updatedCollegeAdmin, _currentUserService.UserId);
            return response.ToActionResult();
        }


        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin")]
        [HttpPut("department_admin", Name = "UpdateDepartmentAdmin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<EmployeeDTO>>> UpdateDepartmentAdmin([FromQuery] EmployeeIdParameter employeeIdParameter, [FromBody] UpdateDepartmentAdminDTO updatedDepartmentAdmin)
        {
            var scope = _currentUserService.ToEmployeeScope();
            var response = await _employeeService.UpdateDepartmentAdmin(scope, employeeIdParameter.EmployeeId, updatedDepartmentAdmin, _currentUserService.UserId);
            return response.ToActionResult();
        }
    }
}
