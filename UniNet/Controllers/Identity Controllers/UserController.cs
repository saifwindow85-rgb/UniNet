using Contracts.Common.Messages;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.UserRequests;
using Contracts.Responses;
using Contracts.Responses.IdentityResponses;
using Domain.Entities.Identity;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Domain.Interfaces.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Extensions;

namespace UniNet.Controllers.Identity_Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;

        public UserController(IUserService userService, ICurrentUserService currentUserService)
        {
            _userService = userService;
            _currentUserService = currentUserService;
        }

        [HttpGet("Id", Name = "GetUserById")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetUserById([FromQuery]IdParameter @param)
        {
            var user = await _userService.FindById(param.Id);
            if (user == null)
                return NotFound(ErrorMessages.NotFound<User>(param.Id));

            return Ok(user);
        }

        [HttpPost("{Id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<UserDTO>>>AddUser(AddUserDTO newUser)
        {
            var userId = _currentUserService.UserId;
            var response = await _userService.AddUser(newUser,userId);
            return response.ToActionResult();
        }

        [HttpPut("{Id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AddUpdateServiceResponse<UserDTO>>> UpdateUser([FromQuery] IdParameter requestParameter,UpdateUserDTO updatedUser)
        {
            var userId = _currentUserService.UserId;
            var response = await _userService.UpdateUser(requestParameter.Id, updatedUser, userId);
            return response.ToActionResult();
        }
    }
}
