using MembershipSystem.Database;
using MembershipSystem.Models;

using Microsoft.AspNetCore.Mvc;

namespace MembershipSystem.Controllers
{
    public class UserRequest
    {
        public string Username {get;set;}
        public string Password { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserRequest request)
        {
            var user = _userService.Authenticate(request.Username, request.Password);

            if (user == null)
            {
                return BadRequest(new ResponseMessage { Message = "Authentication details are invalid"});
            }

            return Ok(user);
        }
    }
}
