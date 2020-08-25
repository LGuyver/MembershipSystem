
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
        public const string NoCompany = "Company does not exist";
        public const string CardExists = "Member card already exists";
        public const string DefaultError = "System error, please contact support";

        public static class HandlePostRequest
        {
            public const int ReturnNoCompany = 1;
            public const int ReturnCardExists = 2;
            public const int ReturnUpdatedDetails = 3;
            public const int ReturnRegisteredDetails = 4;
        }

        public static class HandleGetRequest
        {
            public const int ReturnRegister = 1;
            public const int ReturnInactive = 2;
            public const int ReturnWelcome = 3;
        }
    }
}
