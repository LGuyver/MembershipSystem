using Xunit;
using FluentAssertions;
using MembershipSystem.Models;
using FluentAssertions.Execution;

namespace MembershipSystem.Test
{
    public class MapperTests
    {
        private readonly Mappers _mappers = new Mappers();

        [Fact]
        [Trait("Category", "Unit")]
        public void Mapper_NewCardAndMember_ShouldReturn_DbDataCard()
        {
            var details = SignupRequestDetails();
            var response = _mappers.MapNewCardAndMemberDetails(details, 1);

            using (new AssertionScope())
            {
                response.Should().BeOfType<DbDataCard>();
                response.Member.Should().BeOfType<DbMember>();
            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Mapper_NewCardAndMember_ShouldReturn_Live()
        {
            var details = SignupRequestDetails();
            var response = _mappers.MapNewCardAndMemberDetails(details, 1);

            using (new AssertionScope())
            {
                response.IsLive.Should().Be(true);
                response.Member.IsLive.Should().Be(true);
            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Mapper_UpdateMemberAndCard_ShouldReturn_DbMemberDbDataCard()
        {
            var details = SignupRequestDetails();
            var (member, dataCard) = _mappers.MapUpdatedMemberDetails(details, 1, 1);

            using (new AssertionScope())
            {
                member.Should().BeOfType<DbMember>();
                dataCard.Should().BeOfType<DbDataCard>();
            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Mapper_UpdateMemberAndCard_ShouldSet_MemberId()
        {
            var details = SignupRequestDetails();
            var (member, dataCard) = _mappers.MapUpdatedMemberDetails(details, 1, 1);

            using (new AssertionScope())
            {
                member.Id.Should().Be(1);
                dataCard.MemberId.Should().Be(1);
                dataCard.Member.Should().BeNull();
            }
        }

        private MembershipSignupRequest SignupRequestDetails()
            => new MembershipSignupRequest
            {
                CardId = "1234567890123456",
                EmployeeId = "ABCDE",
                FirstName = "Test",
                LastName = "Guy",
                Email = "test@email.com",
                PhoneNumber = "++34657674524",
                Pin = 1234
            };
    }
}
