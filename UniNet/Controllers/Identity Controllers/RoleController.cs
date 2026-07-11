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
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<RoleDTO>>> GetRoles([FromQuery]PagedResultParameters parameters)
        {
            var roles = await _roleService.GetRoles(parameters.PageNumber, parameters.PageSize);
            return roles.ToPagedActioneResult();
        }

        [Authorize]
        [HttpGet("{id}",Name ="GetRoleById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoleDTO>> GetRoleById([FromQuery]IdParameter @parameter)
        {
            var role = await _roleService.FindRoleDTOById(parameter.Id);
            if (role == null)
                return NotFound(ErrorMessages.NotFound<Role>(parameter.Id));

            return Ok(role);
        }

        [Authorize]
        [HttpGet("name",Name ="GetRoleByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoleDTO>> GetRolebyName([FromQuery]BaseStringParametre parametre)
        {
            var role = await _roleService.FindRoleDTOByRoleName(parametre.Name);

            if (role == null)
                return NotFound(ErrorMessages.NotFound<Role>(parametre.Name));

            return Ok(role);
        }

        [Authorize]
        [HttpDelete("id", Name = "DeleteRoleById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
         public async Task<ActionResult<bool>> DeleteRole([FromQuery]IdParameter @parameter)
        {
            var result = await _roleService.Delete(parameter.Id);
            if(!result)
            {
                return NotFound(DeleteMessage.DeletionFailed<Role>(parameter.Id));
            }

            return Ok(DeleteMessage.DeletedSuccessfuly<Role>(parameter.Id));
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<RoleDTO>>>AddRole(AddRoleDTO newRole)
        {
            var response = await _roleService.AddRole(newRole, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AddUpdateServiceResponse<RoleDTO>>>UpdateRole(AddRoleDTO updatedRole, [FromQuery]IdParameter @parameter)
        {
            var response = await _roleService.UpdateRole(updatedRole, parameter.Id, _currentUserService.UserId);
            return response.ToActionResult();
        }
    }
}
