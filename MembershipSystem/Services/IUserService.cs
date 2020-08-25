
namespace MembershipSystem.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        string GenerateToken(int userId);
    }
}
