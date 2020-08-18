using MembershipSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        public async Task<int> GetDataCardMemberIdAsync(string cardId, CancellationToken token)
        {
            var result = await _membershipContext.DataCards.SingleOrDefaultAsync(x => x.CardId == cardId && x.IsLive, token).ConfigureAwait(false);
            return result?.MemberId ?? 0;
        }

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
    }
}
