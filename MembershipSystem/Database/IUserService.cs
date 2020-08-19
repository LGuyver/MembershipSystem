using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MembershipSystem.Database
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
    }
}
