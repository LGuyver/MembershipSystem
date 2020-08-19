using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using MembershipSystem.Database;
using MembershipSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private IUserService _userService;

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
