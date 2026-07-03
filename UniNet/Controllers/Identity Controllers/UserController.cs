using Contracts.DTOs.UserDTOs;
using Contracts.Messages;
using Contracts.Parameters_Validations;
using Domain.Entities.Identity;
using Domain.Interfaces.UnitOfWork;
using Domain.Interfaces.UserInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("id:int", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetUserById([FromQuery]IdDTO @param)
        {
            var user = await _userService.FindById(param.Id);
            if (user == null)
                return NotFound(ErrorMessages.NotFound<User>(param.Id));

            return Ok(user);
        }
    }
}
