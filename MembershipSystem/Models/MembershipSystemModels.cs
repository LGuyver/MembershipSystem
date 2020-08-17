using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    public class MembershipExistingReponse
    {
        public string CardId { get; set; }
        public MemberDetails Member { get; set; }
    }

}
