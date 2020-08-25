using MembershipSystem.Models;
using MembershipSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace MembershipSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MembershipController : Controller
    {
        private readonly IMembershipService _service;

        public MembershipController(IMembershipService membershipService)
        {
            _service = membershipService;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Get([FromBody] MembershipSystemRequest request, CancellationToken token)
        {
            var (returnId, membershipResponse) = await _service.HandleMembershipGetRequestAsync(request, token).ConfigureAwait(false);
            return returnId switch
            {
                Constants.HandleGetRequest.ReturnRegister
                    => NotFound(membershipResponse),
                Constants.HandleGetRequest.ReturnInactive
                    => BadRequest(new ResponseMessage() { Message = $"{Constants.CardNotInUse}. {Constants.ContactSupport}" }),
                Constants.HandleGetRequest.ReturnWelcome
                    => Ok(membershipResponse),
                _ => BadRequest(new ResponseMessage() { Message = Constants.DefaultError })
            };
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Post([FromBody] MembershipSignupRequest request, CancellationToken token)
        {
            var (returnId, membershipResponse) = await _service.HandleMembershipPostRequestAsync(request, token).ConfigureAwait(false);
            return returnId switch
            {
                Constants.HandlePostRequest.ReturnNoCompany
                    => BadRequest(new ResponseMessage() { Message = $"{Constants.NoCompany}. {Constants.ContactSupport}" }),
                Constants.HandlePostRequest.ReturnCardExists
                    => Conflict(new ResponseMessage() { Message = Constants.CardExists }),
                Constants.HandlePostRequest.ReturnUpdatedDetails
                    => Created("Membership", membershipResponse),
                Constants.HandlePostRequest.ReturnRegisteredDetails
                    => Created("Membership", membershipResponse),
                _ => BadRequest(new ResponseMessage() { Message = Constants.DefaultError })
            };
        }
    }
}