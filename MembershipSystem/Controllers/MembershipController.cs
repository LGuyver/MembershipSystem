using MembershipSystem.Database;
using MembershipSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace MembershipSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MembershipController : Controller
    {
        private readonly IMembershipRepository _membershipRepository;

        public MembershipController(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Get([FromBody] MembershipSystemRequest request, CancellationToken token)
        {
            var memberId = await _membershipRepository.GetDataCardMemberIdAsync(request.CardId, token);
            if (memberId != 0)
            {
                var member = await _membershipRepository.GetMemberDetailsAsync(memberId, token);
                var memberDetails = new MemberDetails()
                {
                    Id = member.Id,
                    Name = $"{member.FirstName} {member.LastName}",
                };
                return StatusCode((int)HttpStatusCode.OK,
                    Json(new MembershipExistingReponse() { CardId = request.CardId, Member = memberDetails }));
            }
            return StatusCode((int)HttpStatusCode.Accepted,
                Json("Please register"));
        }
    }
}
