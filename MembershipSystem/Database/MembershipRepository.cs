using MembershipSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace MembershipSystem.Database
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly MembershipContext _membershipContext;

        public MembershipRepository(MembershipContext membershipContext)
        {
            _membershipContext = membershipContext;
        }

        public async Task<DbDataCard> GetDataCardAsync(string cardId, CancellationToken token)
            => await _membershipContext.DataCards.SingleOrDefaultAsync(x => x.CardId == cardId, token).ConfigureAwait(false);

        public async Task<DbMember> GetMemberDetailsAsync(int memberId, CancellationToken token)
            => await _membershipContext.Members.SingleAsync(x => x.Id == memberId, token).ConfigureAwait(false);

        public async Task<int> GetCompanyIdAsync(string companyName, CancellationToken token)
        {
            var result = await _membershipContext.Companies.SingleOrDefaultAsync(x => x.Name == companyName && x.IsLive, token).ConfigureAwait(false);
            return result?.Id ?? 0;
        }

        public async Task AddCardAndMemberAsync(DbDataCard memberCardDetails, CancellationToken token)
        {
            await _membershipContext.DataCards.AddAsync(memberCardDetails, token).ConfigureAwait(false);
            await _membershipContext.SaveChangesAsync(token).ConfigureAwait(false);
        }

        public async Task<DbMember> GetRegisteredMember(string employeeId, CancellationToken token) 
            => await _membershipContext.Members.SingleOrDefaultAsync(x => x.EmployeeId == employeeId, token).ConfigureAwait(false);

        public async Task UpdateMemberAndCreateCardAsync(DbDataCard memberCardDetails, DbMember memberDetails, CancellationToken token)
        {
            var member = await _membershipContext.Members.SingleOrDefaultAsync(x => x.Id == memberDetails.Id, token).ConfigureAwait(false);
            member.FirstName = memberDetails.FirstName;
            member.LastName = memberDetails.LastName;
            member.Email = memberDetails.Email;
            member.PhoneNumber = memberDetails.PhoneNumber;
            member.SecurityPin = memberDetails.SecurityPin;
            member.CompanyId = memberDetails.CompanyId;

            _membershipContext.Members.Update(member);
            await _membershipContext.DataCards.AddAsync(memberCardDetails, token).ConfigureAwait(false);
            await _membershipContext.SaveChangesAsync(token).ConfigureAwait(false);
        }

        public async Task DeactivateMembersPreviousCardAsync(int memberId, CancellationToken token)
        {
            var result = await _membershipContext.DataCards.SingleOrDefaultAsync(x => x.MemberId == memberId && x.IsLive, token).ConfigureAwait(false);
            if (result != null)
            {
                result.IsLive = false;
                _membershipContext.DataCards.Update(result);
                await _membershipContext.SaveChangesAsync(token).ConfigureAwait(false);
            }
        }
    }
}
