using MembershipSystem.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MembershipSystem.Services
{
    public interface IMembershipService
    {
        Task<(int returnId, MembershipReponse membershipResponse)> HandleMembershipGetRequestAsync(MembershipSystemRequest request, CancellationToken token);
        Task<(int returnId, MembershipReponse membershipResponse)> HandleMembershipPostRequestAsync(MembershipSignupRequest request, CancellationToken token);
    }
}