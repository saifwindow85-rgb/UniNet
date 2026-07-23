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
        [Authorize(Roles ="Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetUserById([FromQuery]UserIdParameter userIdparameter)
        {
            var user = await _userService.FindById(userIdparameter.UserId);
            return user.GetResourceEndpoints(userIdparameter.UserId, typeof(User).Name);
        }

        [HttpPost("")]
        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AddUpdateServiceResponse<UserDTO>>>AddUser(AddUserDTO newUser)
        {
            var userId = _currentUserService.UserId;
            var response = await _userService.AddUser(newUser,userId);
            return response.ToActionResult();
        }

        [HttpPut("")]
        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AddUpdateServiceResponse<UserDTO>>> UpdateUser([FromQuery] UserIdParameter userIdparameter,UpdateUserDTO updatedUser)
        {
            var userId = _currentUserService.UserId;
            var response = await _userService.UpdateUser(userIdparameter.UserId, updatedUser, userId);
            return response.ToActionResult();
        }
    }
}
