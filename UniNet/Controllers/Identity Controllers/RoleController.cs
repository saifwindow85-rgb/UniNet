using Contracts.Common.Messages;
using Contracts.Requests.IdentityRequests.RoleRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses;
using Contracts.Responses.IdentityResponses.RoleResponses;
using Contracts.Results;
using Domain.Entities.Identity;
using Domain.Interfaces.IdentityInterfaces.RoleInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using UniNet.Extensions;

namespace UniNet.Controllers.Identity_Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ICurrentUserService _currentUserService;
        public RoleController(IRoleService roleService,ICurrentUserService currentUserService)
        {
            _roleService = roleService;
            _currentUserService = currentUserService;
        }

        [Authorize]
        [HttpGet(Name ="GetRoles")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<RoleDTO>>> GetRoles([FromQuery]PagedResultParameters parameters)
        {
            var roles = await _roleService.GetRoles(parameters.PageNumber, parameters.PageSize);
            return roles.ToPagedActioneResult();
        }

        [Authorize]
        [HttpGet("by-id",Name ="GetRoleById")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoleDTO>> GetRoleById([FromQuery]RoleIdParameter roleIdParameter)
        {
            var role = await _roleService.FindRoleDTOById(roleIdParameter.RoleId);
            return role.GetResourceEndpoints(roleIdParameter.RoleId, typeof(Role).Name);
        }

        [Authorize]
        [HttpGet("by-name",Name ="GetRoleByName")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoleDTO>> GetRolebyName([FromQuery]BaseStringParametre parametre)
        {
            var role = await _roleService.FindRoleDTOByRoleName(parametre.Name);
            return role.GetResourceEndpoints(parametre.Name, typeof(Role).Name);
        }

        [Authorize]
        [HttpDelete( Name = "DeleteRoleById")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<bool>> DeleteRole([FromQuery]RoleIdParameter roleIdParameter)
        {
            var result = await _roleService.Delete(roleIdParameter.RoleId);
            return result.ToDeleteActionResult<Role>(roleIdParameter.RoleId);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AddUpdateServiceResponse<RoleDTO>>>AddRole(AddRoleDTO newRole)
        {
            var response = await _roleService.AddRole(newRole);
            return response.ToActionResult();
        }

        [Authorize]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AddUpdateServiceResponse<RoleDTO>>>UpdateRole(AddRoleDTO updatedRole, [FromQuery]RoleIdParameter roleIdParameter)
        {
            var response = await _roleService.UpdateRole(updatedRole, roleIdParameter.RoleId);
            return response.ToActionResult();
        }
    }
}
