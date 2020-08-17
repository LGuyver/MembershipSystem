using MembershipSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MembershipSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MembershipController : Controller
    {
        [HttpGet]
        public ActionResult Get([FromBody] MembershipSystemRequest request)
        {
            if (request.CardId != " ")
            {
                return StatusCode((int)HttpStatusCode.OK);
            }
            return StatusCode((int)HttpStatusCode.BadRequest);
        }
    }
}
