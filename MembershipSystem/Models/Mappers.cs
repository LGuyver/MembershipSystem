using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MembershipSystem.Models
{
    public class Mappers
    {
        public DbDataCard MapNewCardAndMemberDetails(MembershipSignupRequest memberDetails, int companyId)
        {
            return new DbDataCard
            {
                CardId = memberDetails.CardId,
                IsLive = true,
                Member = new DbMember
                {
                    EmployeeId = memberDetails.EmployeeId,
                    FirstName = memberDetails.FirstName,
                    LastName = memberDetails.LastName,
                    Email = memberDetails.Email,
                    PhoneNumber = memberDetails.PhoneNumber,
                    SecurityPin = memberDetails.Pin,
                    CompanyId = companyId,
                    IsLive = true,
                }
            };
        }
        public (DbMember member, DbDataCard dataCard) MapUpdatedMemberDetails(MembershipSignupRequest memberDetails, int existingMemberId, int companyId)
        {
            var member = new DbMember
            {
                Id = existingMemberId,
                EmployeeId = memberDetails.EmployeeId,
                FirstName = memberDetails.FirstName,
                LastName = memberDetails.LastName,
                Email = memberDetails.Email,
                PhoneNumber = memberDetails.PhoneNumber,
                SecurityPin = memberDetails.Pin,
                CompanyId = companyId,
                IsLive = true,
            };

            var dataCard = new DbDataCard
            {
                MemberId = existingMemberId,
                CardId = memberDetails.CardId,
                IsLive = true,
            };

            return (member, dataCard);
        }
    }
}
