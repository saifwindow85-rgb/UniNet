using Contracts.Messages;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.UserRequests;
using Contracts.Responses;
using Domain.Entities.Identity;
using Domain.Interfaces.UnitOfWork;
using Domain.Interfaces.UserInterfaces;
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
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("Id", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetUserById([FromQuery]RequestParameters @param)
        {
            var user = await _userService.FindById(param.Id);
            if (user == null)
                return NotFound(ErrorMessages.NotFound<User>(param.Id));

            return Ok(user);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddUpdateServiceResponse<UserDTO>>>AddUser(AddUserDTO newUser)
        {
            var response = await _userService.AddUser(newUser);
            return response.ToActionResult();
        }
    }
}
