using MembershipSystem.Database;
using MembershipSystem.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MembershipSystem.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly Mappers _mappers;

        public MembershipService(IMembershipRepository membershipRepository, Mappers mappers)
        {
            _membershipRepository = membershipRepository;
            _mappers = mappers;
        }

        public async Task<(int returnId, MembershipReponse membershipResponse)> HandleMembershipGetRequestAsync(MembershipSystemRequest request, CancellationToken token)
        {
            var dataCard = await _membershipRepository.GetDataCardAsync(request.CardId, token).ConfigureAwait(false);
            if (dataCard == null)
            {
                // The card provided does not exist and should now be sent to register for an account
                return (Constants.HandleGetRequest.ReturnRegister, new MembershipReponse() { Message = Constants.Register, CardId = request.CardId });
            }

            if (!dataCard.IsLive)
            {
                // The card provided is no longer active
                return (Constants.HandleGetRequest.ReturnInactive, null);
            }

            var member = await _membershipRepository.GetMemberDetailsAsync(dataCard.MemberId, token).ConfigureAwait(false);
            var memberDetails = FormatMembeDetails(member.Id, member.FirstName, member.LastName);

            // Card is already registered and active, member is welcomed
            return (Constants.HandleGetRequest.ReturnWelcome, new MembershipReponse() { Message = Constants.Welcome, CardId = request.CardId, Member = memberDetails });
        }

        public async Task<(int returnId, MembershipReponse membershipResponse)> HandleMembershipPostRequestAsync(MembershipSignupRequest request, CancellationToken token)
        {
            var existingDataCard = await _membershipRepository.GetDataCardAsync(request.CardId, token).ConfigureAwait(false);
            if (existingDataCard != null)
            {
                return (Constants.HandlePostRequest.ReturnCardExists, null);
            }

            var companyId = await _membershipRepository.GetCompanyIdAsync(request.Company, token).ConfigureAwait(false);
            if (companyId == 0)
            {
                return (Constants.HandlePostRequest.ReturnNoCompany, null);
            }

            var existingMember = await _membershipRepository.GetRegisteredMember(request.EmployeeId, token).ConfigureAwait(false);
            if (existingMember != null)
            {
                // The employee is registered but the card needs to be linked to account

                var updatedDetails = await UpdateMember(request, existingMember.Id, companyId, token).ConfigureAwait(false);
                return (Constants.HandlePostRequest.ReturnUpdatedDetails, updatedDetails);
            }

            var newRegisteredDetails = await RegisterMember(request, companyId, token).ConfigureAwait(false);
            return (Constants.HandlePostRequest.ReturnRegisteredDetails, newRegisteredDetails);
        }

        private async Task<MembershipReponse> UpdateMember(MembershipSignupRequest request, int existingMemberId, int companyId, CancellationToken token)
        {
            var (member, dataCard) = _mappers.MapUpdatedMemberDetails(request, existingMemberId, companyId);

            // Deactive the previous data card if member had one
            await _membershipRepository.DeactivateMembersPreviousCardAsync(existingMemberId, token).ConfigureAwait(false);

            // Update the members details with the recent input, and link to the new data card
            await _membershipRepository.UpdateMemberAndCreateCardAsync(dataCard, member, token).ConfigureAwait(false);

            var memberDetails = FormatMembeDetails(existingMemberId, member.FirstName, member.LastName);


            return new MembershipReponse() { Message = Constants.LinkedNewCard, CardId = dataCard.CardId, Member = memberDetails };
        }

        private async Task<MembershipReponse> RegisterMember(MembershipSignupRequest request, int companyId, CancellationToken token)
        {
            var newData = _mappers.MapNewCardAndMemberDetails(request, companyId);
            await _membershipRepository.AddCardAndMemberAsync(newData, token).ConfigureAwait(false);

            var memberDetails = FormatMembeDetails(newData.Member.Id, newData.Member.FirstName, newData.Member.LastName);

            return new MembershipReponse() { Message = Constants.Registered, CardId = newData.CardId, Member = memberDetails };
        }

        private MemberDetails FormatMembeDetails(int id, string firstName, string lastName) 
            => new MemberDetails()
            {
                Id = id,
                Name = $"{firstName} {lastName}",
            };
    }
}
