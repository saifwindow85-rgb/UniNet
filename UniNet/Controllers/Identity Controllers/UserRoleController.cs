using Contracts.Common.Messages;
using Contracts.Requests.IdentityRequests.UserRoleRequsets;
using Contracts.Requests.RequestParameters;
using Contracts.Responses;
using Contracts.Responses.IdentityResponses.UserRoleResponse;
using Contracts.Results;
using Domain.Entities.Identity;
using Domain.Interfaces.IdentityInterfaces.UserRoleInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Extensions;

namespace UniNet.Controllers.Identity_Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;
        public UserRoleController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        [HttpGet(Name ="GetAllUserUserRoles")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<UserRoleDTO>>> GetAllUserRoles([FromQuery]PagedResultParameters @parameter)
        {
            return await _userRoleService.GetAllUserRole(parameter.PageNumber, parameter.PageSize);
        }

        [HttpGet("roleid",Name = "GetFiltredUserRoles")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<UserRoleDTO>>> GetUserRolesPerRoleId([FromQuery] PagedResultParameters @parameter,RoleIdParameter roleIdParameter)
        {
            return await _userRoleService.GetUserRolesPerRoleId(roleIdParameter.RoleId, parameter.PageNumber, parameter.PageSize);
        }

        [HttpGet("by-id",Name ="GetUserRoleById")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserRoleDTO?>> GetUserRoleById([FromQuery]UserRoleParameters @parameter)
        {
            var userRole =  await _userRoleService.FindUserRoleDTOById(parameter.UserId,parameter.RoleId);
            if (userRole == null)
                return NotFound($"UserRole with ids({parameter.UserId},{parameter.RoleId}) NotFound!");
            return Ok(userRole);
        }

        [HttpDelete(Name ="DeleteUserRole")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<bool>> Delete([FromQuery]UserRoleParameters @parameter)
        {
            var result = await _userRoleService.Delete(parameter.UserId, parameter.RoleId);
            if (!result)
                return NotFound($"UserRole with ids({parameter.UserId},{parameter.RoleId}) NotFound!");

            return NoContent();
        }

        [HttpPost(Name ="AddUserRole")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AddUpdateServiceResponse<UserRoleDTO>>>AddUserRole(AddUserRoleDTO userRoleDTO)
        {
            var response = await _userRoleService.AddUserRole(userRoleDTO);
            return response.ToActionResult();
        }
    }
}
