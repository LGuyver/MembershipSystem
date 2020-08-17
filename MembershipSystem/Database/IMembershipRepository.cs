﻿
using MembershipSystem.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MembershipSystem.Database
{
    public interface IMembershipRepository
    {
        Task<int> GetDataCardMemberIdAsync(string cardId, CancellationToken token);
        Task<DbMember> GetMemberDetailsAsync(int memberId, CancellationToken token);
    }
}
