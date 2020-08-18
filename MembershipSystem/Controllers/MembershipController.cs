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
            var memberId = await _membershipRepository.GetDataCardMemberIdAsync(request.CardId, token).ConfigureAwait(false);
            if (memberId != 0)
            {
                var member = await _membershipRepository.GetMemberDetailsAsync(memberId, token).ConfigureAwait(false);
                var memberDetails = new MemberDetails()
                {
                    Id = member.Id,
                    Name = $"{member.FirstName} {member.LastName}",
                };
                return StatusCode((int)HttpStatusCode.OK,
                    Json("Welcome", new MembershipExistingReponse() { CardId = request.CardId, Member = memberDetails }));
            }
            return StatusCode((int)HttpStatusCode.Accepted,
                Json("Please register")); // fix these returns 
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Post([FromBody] MembershipSignupRequest request, CancellationToken token)
        {
            var companyId = await _membershipRepository.GetCompanyIdAsync(request.Company, token).ConfigureAwait(false);
            if (companyId == 0)
            {
                return BadRequest("Company does not exist, please contact support.");
            }
            var newMember = MapMemberDetails(request, companyId);
            await _membershipRepository.AddMemberAsync(newMember, token).ConfigureAwait(false);

            var memberDetails = new MemberDetails()
            {
                Id = newMember.Id,
                Name = $"{newMember.FirstName} {newMember.LastName}",
            };

            return StatusCode((int)HttpStatusCode.Created,
                    Json("Registered", new MembershipExistingReponse() { CardId = newMember.LinkedDataCard.CardId, Member = memberDetails }));
        }

        private DbMember MapMemberDetails(MembershipSignupRequest memberDetails, int companyId)
        {
            return new DbMember
            {
                EmployeeId = memberDetails.EmployeeId,
                FirstName = memberDetails.FirstName,
                LastName = memberDetails.LastName,
                Email = memberDetails.Email,
                PhoneNumber = memberDetails.PhoneNumber,
                SecurityPin = memberDetails.Pin,
                CompanyId = companyId,
                IsLive = true,
                LinkedDataCard = new DbDataCard
                {
                    CardId = memberDetails.CardId,
                    IsLive = true
                }
            };
        }
    }
}
