using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MembershipSystem.Models
{
    public static class Constants
    {
        public const string Register = "Please register";
        public const string Welcome = "Welcome";
        public const string Registered = "Registered";
        public const string LinkedNewCard = "Linked card to existing member";
        public const string ContactSupport = "Please contact support";
        public const string CardNotInUse = "Member card should not be in use";
        public const string NoCompnay = "Company does not exist";
        public const string CardExists = "Member card already exists";

        public static class HandleRequest
        {
            public const int ReturnNoCompany = 1;
            public const int ReturnCardExists = 2;
            public const int ReturnUpdatedDetails = 3;
            public const int ReturnRegisteredDetails = 4;
        }
    }
}
