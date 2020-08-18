using MembershipSystem.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MembershipSystem.Database
{
    public interface IMembershipRepository
    {
        Task<DbDataCard> GetDataCardAsync(string cardId, CancellationToken token);
        Task<DbMember> GetMemberDetailsAsync(int memberId, CancellationToken token);
        Task<int> GetCompanyIdAsync(string companyName, CancellationToken token);
        Task AddCardAndMemberAsync(DbDataCard memberCardDetails, CancellationToken token);
        Task<DbMember> GetRegisteredMember(string employeeId, CancellationToken token);
        Task UpdateMemberAndCreateCardAsync(DbDataCard memberCardDetails, DbMember memberDetails, CancellationToken token);
        Task DeactivateMembersPreviousCardAsync(int memberId, CancellationToken token);
    }
}
