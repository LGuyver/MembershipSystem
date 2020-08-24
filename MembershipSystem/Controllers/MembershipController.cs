using MembershipSystem.Database;
using MembershipSystem.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design.Serialization;
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
        private readonly Mappers _mappers;
        private readonly MembershipService _service;

        public MembershipController(IMembershipRepository membershipRepository, Mappers mappers, MembershipService membershipService)
        {
            _membershipRepository = membershipRepository;
            _mappers = mappers;
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
            var dataCard = await _membershipRepository.GetDataCardAsync(request.CardId, token).ConfigureAwait(false);

            if (dataCard == null)
            {
                // The card provided does not exist and should now be sent to register for an account
                return NotFound(new MembershipReponse() { Message = Constants.Register, CardId = request.CardId });
            }

            if (!dataCard.IsLive)
            {
                // The card provided is no longer active
                return BadRequest(new ResponseMessage() { Message = $"{Constants.CardNotInUse}. {Constants.ContactSupport}" });
            }

            var member = await _membershipRepository.GetMemberDetailsAsync(dataCard.MemberId, token).ConfigureAwait(false);
            var memberDetails = new MemberDetails()
            {
                Id = member.Id,
                Name = $"{member.FirstName} {member.LastName}",
            };
            // Card is already registered and active, member is welcomed
            return Ok(new MembershipReponse() { Message = Constants.Welcome, CardId = request.CardId, Member = memberDetails });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Post([FromBody] MembershipSignupRequest request, CancellationToken token)
        {
            var (returnId, response) = await _service.HandlePostRequest(request, token).ConfigureAwait(false);
            return returnId switch
            {
                Constants.HandleRequest.ReturnNoCompany
                    => BadRequest(new ResponseMessage() { Message = $"{Constants.NoCompnay}. {Constants.ContactSupport}" }),
                Constants.HandleRequest.ReturnCardExists
                    => Conflict(new ResponseMessage() { Message = Constants.CardExists }),
                Constants.HandleRequest.ReturnUpdatedDetails
                    => Created("Membership", response),
                Constants.HandleRequest.ReturnRegisteredDetails
                    => Created("Membership", response),
                _ => BadRequest(),
            };
        }
    }
}


            ////var companyId = await _membershipRepository.GetCompanyIdAsync(request.Company, token).ConfigureAwait(false);
            //var companyId = await _service.HandlePostRequest(request, token);
            //if (companyId == 0)
            //{
            //    // The company provided is not expected or registered
            //    return BadRequest(new ResponseMessage() { Message = $"{Constants.NoCompnay}. {Constants.ContactSupport}" });
            //}

            //var existingDataCard = await _membershipRepository.GetDataCardAsync(request.CardId, token).ConfigureAwait(false);
            //if (existingDataCard != null)
            //{
            //    // The card provided is already registered, member should not be registering
            //    return Conflict(new ResponseMessage() { Message = Constants.CardExists});
            //}

            //var existingMember = await _membershipRepository.GetRegisteredMember(request.EmployeeId, token).ConfigureAwait(false);
            //if (existingMember != null)
            //{
            //    // The employee is registered but the card needs to be linked to account
            //    var (member, dataCard) = _mappers.MapUpdatedMemberDetails(request, existingMember.Id, companyId);

            //    // Deactive the previous data card if member had one
            //    await _membershipRepository.DeactivateMembersPreviousCardAsync(existingMember.Id, token).ConfigureAwait(false);

            //    // Update the members details with the recent input, and link to the new data card
            //    await _membershipRepository.UpdateMemberAndCreateCardAsync(dataCard, member, token).ConfigureAwait(false);

            //    var updatedMemberDetails = new MemberDetails()
            //    {
            //        Id = existingMember.Id,
            //        Name = $"{member.FirstName} {member.LastName}",
            //    };

            //    return Created("Membership", new MembershipReponse() { Message = Constants.LinkedNewCard, CardId = dataCard.CardId, Member = updatedMemberDetails });
            //}

            //// Data card and member are new and need to be registered
            //var newData = _mappers.MapNewCardAndMemberDetails(request, companyId);
            //await _membershipRepository.AddCardAndMemberAsync(newData, token).ConfigureAwait(false);
            //var memberDetails = new MemberDetails()
            //{
            //    Id = newData.Member.Id,
            //    Name = $"{newData.Member.FirstName} {newData.Member.LastName}",
            //};
            //return Created("Membership", new MembershipReponse() { Message = Constants.Registered, CardId = newData.CardId, Member = memberDetails });