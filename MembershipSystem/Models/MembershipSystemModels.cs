﻿
namespace MembershipSystem.Models
{
    public class MembershipSystemRequest
    {
        public string CardId { get; set; }
    }

    public class MemberDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ResponseMessage
    {
        public string Message { get; set; }
    }

    public class MembershipReponse
    {
        public string Message { get; set; }
        public string CardId { get; set; }
        public MemberDetails Member { get; set; }
    }

    public class MembershipSignupRequest
    {
        public string CardId { get; set; }
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Company { get; set; }
        public int Pin { get; set; }
    }
}
