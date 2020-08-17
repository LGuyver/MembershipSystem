using MembershipSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MembershipSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MembershipController : Controller
    {
        [HttpPost]
        public ActionResult Post([FromBody] requestModel request)
        {
            if (request.Request != 0)
            {
                return StatusCode((int)HttpStatusCode.OK);
            }
            return StatusCode((int)HttpStatusCode.BadRequest);
        }
    }
}
