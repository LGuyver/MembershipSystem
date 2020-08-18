
namespace MembershipSystem.Models
{
    public class DbDataCard
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string CardId { get; set; }
        public bool IsLive { get; set; }
        public virtual DbMember Member { get; set; }

    }

    public class DbMember
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int SecurityPin { get; set; }
        public int CompanyId { get; set; }
        public bool IsLive { get; set; }
    }

    public class DbCompany
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsLive { get; set; }
    }

}
