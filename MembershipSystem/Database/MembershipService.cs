using FluentValidation.Validators;
using MembershipSystem.Models;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MembershipSystem.Database
{
    public class MembershipService
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly Mappers _mappers;

        public MembershipService(IMembershipRepository membershipRepository, Mappers mappers)
        {
            _membershipRepository = membershipRepository;
            _mappers = mappers;
        }

        public async Task<(int returnId, MembershipReponse response)> HandlePostRequest(MembershipSignupRequest request, CancellationToken token)
        {
            var existingDataCard = await _membershipRepository.GetDataCardAsync(request.CardId, token).ConfigureAwait(false);
            if (existingDataCard != null)
            {
                return (Constants.HandleRequest.ReturnCardExists, null);
            }

            var companyId = await _membershipRepository.GetCompanyIdAsync(request.Company, token).ConfigureAwait(false);
            if (companyId == 0)
            {
                return (Constants.HandleRequest.ReturnNoCompany, null);
            }

            var existingMember = await _membershipRepository.GetRegisteredMember(request.EmployeeId, token).ConfigureAwait(false);
            if (existingMember != null)
            {
                var updatedDetails = await UpdateMember(request, existingMember.Id, companyId, token).ConfigureAwait(false);
                return (Constants.HandleRequest.ReturnUpdatedDetails, updatedDetails);
            }

            var newRegisteredDetails = await RegisterMember(request, companyId, token).ConfigureAwait(false);
            return (Constants.HandleRequest.ReturnRegisteredDetails, newRegisteredDetails);
        }

        private async Task<MembershipReponse> UpdateMember(MembershipSignupRequest request, int existingMemberId, int companyId, CancellationToken token)
        {
            // The employee is registered but the card needs to be linked to account
            var (member, dataCard) = _mappers.MapUpdatedMemberDetails(request, existingMemberId, companyId);

            // Deactive the previous data card if member had one
            await _membershipRepository.DeactivateMembersPreviousCardAsync(existingMemberId, token).ConfigureAwait(false);

            // Update the members details with the recent input, and link to the new data card
            await _membershipRepository.UpdateMemberAndCreateCardAsync(dataCard, member, token).ConfigureAwait(false);

            var memberDetails = new MemberDetails()
            {
                Id = existingMemberId,
                Name = $"{member.FirstName} {member.LastName}",
            };

            return new MembershipReponse() { Message = Constants.LinkedNewCard, CardId = dataCard.CardId, Member = memberDetails };

        }

        private async Task<MembershipReponse> RegisterMember(MembershipSignupRequest request, int companyId, CancellationToken token)
        {
            var newData = _mappers.MapNewCardAndMemberDetails(request, companyId);
            await _membershipRepository.AddCardAndMemberAsync(newData, token).ConfigureAwait(false);

            var memberDetails = new MemberDetails()
            {
                Id = newData.Member.Id,
                Name = $"{newData.Member.FirstName} {newData.Member.LastName}",
            };

            return new MembershipReponse() { Message = Constants.Registered, CardId = newData.CardId, Member = memberDetails };

        }
    }
}
