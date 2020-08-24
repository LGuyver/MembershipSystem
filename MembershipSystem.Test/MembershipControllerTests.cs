using System.Threading.Tasks;
using Xunit;
using Alba;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MembershipSystem.Test.Setup;
using MembershipSystem.Database;
using MembershipSystem.Models;
using FluentAssertions.Execution;

namespace MembershipSystem.Test
{
    public class MembershipControllerTests : IClassFixture<ApplicationFixture>
    {
        private readonly SystemUnderTest _system;
        private readonly MembershipContext _membershipContext;

        private readonly IConfiguration _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        private readonly string _token;
        private readonly IUserService _userServie = new UserService();

        public MembershipControllerTests(ApplicationFixture fixture)
        {
            _system = fixture.SystemUnderTest;
            
            var connectionString = _config.GetValue<string>("DbConnectionString");

            var optionsBuilder = new DbContextOptionsBuilder<MembershipContext>();
            optionsBuilder.UseSqlServer(connectionString);

            _membershipContext = new MembershipContext(optionsBuilder.Options);

            _token = _userServie.GenerateToken(1);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Get_ShouldReturn_UnAuthorized()
        {
            await _system.Scenario(x =>
            {
                x.Get
                    .Json(new MembershipSystemRequest
                    {
                        CardId = "A65gtY2Enm14AwS0"
                    })
                    .ToUrl("/Membership");
                x.StatusCodeShouldBe(StatusCodes.Status401Unauthorized);
            }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Get_ShouldReturn_NotFound_RegisterResponse()
        {
            var result = await _system.Scenario(x =>
            {
                x.SetRequestHeader("Authorization", $"Bearer {_token}");
                x.Get
                    .Json(new MembershipSystemRequest
                    {
                        CardId = "A65gtY2Enm14AwS0"
                    })
                    .ToUrl("/Membership");
                x.StatusCodeShouldBe(StatusCodes.Status404NotFound);
            }).ConfigureAwait(false);

            var response = result.ResponseBody.ReadAsJson<MembershipReponse>();
            using (new AssertionScope())
            {
                response.Message.Should().Be("Please register");
                response.CardId.Should().Be("A65gtY2Enm14AwS0");
                response.Member.Should().BeNull();
            }
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Get_ShouldReturn_Ok_WelcomeResponse()
        {
            PopulateTestData_MemberAndCard(true);
            var result = await _system.Scenario(x =>
            {
                x.SetRequestHeader("Authorization", $"Bearer {_token}");
                x.Get
                    .Json(new MembershipSystemRequest
                    {
                        CardId = "74HytRR87mNJ10pl"
                    })
                    .ToUrl("/Membership");
                x.StatusCodeShouldBe(StatusCodes.Status200OK);
            }).ConfigureAwait(false);

            var response = result.ResponseBody.ReadAsJson<MembershipReponse>();
            using (new AssertionScope())
            {
                response.Message.Should().Be("Welcome");
                response.CardId.Should().Be("74HytRR87mNJ10pl");
                response.Member.Should().NotBeNull();
                response.Member.Name.Should().Be("Guy Test");
            }

            CleanupCreatedData("74HytRR87mNJ10pl");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Get_ShouldReturn_BadRequest_NotActiveResponse()
        {
            PopulateTestData_MemberAndCard(false);
            var result = await _system.Scenario(x =>
            {
                x.SetRequestHeader("Authorization", $"Bearer {_token}");
                x.Get
                    .Json(new MembershipSystemRequest
                    {
                        CardId = "74HytRR87mNJ10pl"
                    })
                    .ToUrl("/Membership");
                x.StatusCodeShouldBe(StatusCodes.Status400BadRequest);
            }).ConfigureAwait(false);

            var response = result.ResponseBody.ReadAsJson<ResponseMessage>();
            response.Message.Should().Be("Member card should not be in use. Please contact support");

            CleanupCreatedData("74HytRR87mNJ10pl");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Get_ShouldReturn_BadRequest_ValidationResponse()
        {
            await _system.Scenario(x =>
            {
                x.SetRequestHeader("Authorization", $"Bearer {_token}");
                x.Get
                    .Json(new MembershipSystemRequest
                    {
                        CardId = "1234"
                    })
                    .ToUrl("/Membership");
                x.StatusCodeShouldBe(StatusCodes.Status400BadRequest);
            }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Post_ShouldReturn_UnAuthorized()
        {
            await _system.Scenario(x =>
            {
                x.Post
                    .Json(new MembershipSignupRequest
                    {
                        CardId = "74HytRR87mNJ10pl",
                        EmployeeId = "111",
                        FirstName = "Guy",
                        LastName = "Test",
                        Email = "testguy@email.com",
                        PhoneNumber = "903546380124",
                        Company = "Test Company",
                        Pin = 1466
                    })
                    .ToUrl("/Membership");
                x.StatusCodeShouldBe(StatusCodes.Status401Unauthorized);
            }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Post_ShouldReturn_BadRequestResponse()
        {
            await _system.Scenario(x =>
            {
                x.SetRequestHeader("Authorization", $"Bearer {_token}");
                x.Post
                    .Json(new MembershipSignupRequest
                    {
                        CardId = "A65gtY2Enm14AwS0",
                        EmployeeId = "111",
                        FirstName = "Guy",
                        LastName = "Test",
                        Email = "testguy@email.com",
                        PhoneNumber = "903546380124",
                        Company = "Test Company DOES NOT EXIST",
                        Pin = 1466
                    })
                    .ToUrl("/Membership");
                x.StatusCodeShouldBe(StatusCodes.Status400BadRequest);
            }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Post_ShouldReturn_Conflict_AlreadyRegisteredResponse()
        {
            PopulateTestData_MemberAndCard(true);

            var result = await _system.Scenario(x =>
            {
                x.SetRequestHeader("Authorization", $"Bearer {_token}");
                x.Post
                    .Json(new MembershipSignupRequest
                    {
                        CardId = "74HytRR87mNJ10pl",
                        EmployeeId = "111",
                        FirstName = "Guy",
                        LastName = "Test",
                        Email = "testguy@email.com",
                        PhoneNumber = "903546380124",
                        Company = "Test Company",
                        Pin = 1466
                    })
                    .ToUrl("/Membership");
                x.StatusCodeShouldBe(StatusCodes.Status409Conflict);
            }).ConfigureAwait(false);

            var response = result.ResponseBody.ReadAsJson<ResponseMessage>();
            response.Message.Should().Be("Member card already exists");

            CleanupCreatedData("74HytRR87mNJ10pl");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Post_ShouldReturn_Created_LinkedCardResponse()
        {
            PopulateTestData_Member();

            var result = await _system.Scenario(x =>
            {
                x.SetRequestHeader("Authorization", $"Bearer {_token}");
                x.Post
                    .Json(new MembershipSignupRequest
                    {
                        CardId = "Aa65egw4uywGE3ty",
                        EmployeeId = "111",
                        FirstName = "Guy",
                        LastName = "Test",
                        Email = "testguy@email.com",
                        PhoneNumber = "903546380124",
                        Company = "Test Company",
                        Pin = 1466
                    })
                    .ToUrl("/Membership");
                x.StatusCodeShouldBe(StatusCodes.Status201Created);
            }).ConfigureAwait(false);

            var response = result.ResponseBody.ReadAsJson<MembershipReponse>();
            using (new AssertionScope())
            {
                response.Message.Should().Be("Linked card to existing member");
                response.CardId.Should().Be("Aa65egw4uywGE3ty");
                response.Member.Name.Should().Be("Guy Test");
            }

            CleanupCreatedData("Aa65egw4uywGE3ty");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Post_ShouldReturn_Created_RegisteredResponse()
        {
            var result = await _system.Scenario(x =>
            {
                x.SetRequestHeader("Authorization", $"Bearer {_token}");
                x.Post
                    .Json(new MembershipSignupRequest
                    {
                        CardId = "Aa65egw4uywGE3ty",
                        EmployeeId = "111",
                        FirstName = "Guy",
                        LastName = "Test",
                        Email = "testguy@email.com",
                        PhoneNumber = "903546380124",
                        Company = "Test Company",
                        Pin = 1466
                    })
                    .ToUrl("/Membership");
                x.StatusCodeShouldBe(StatusCodes.Status201Created);
            }).ConfigureAwait(false);

            var response = result.ResponseBody.ReadAsJson<MembershipReponse>();
            using (new AssertionScope())
            {
                response.Message.Should().Be("Registered");
                response.CardId.Should().Be("Aa65egw4uywGE3ty");
                response.Member.Name.Should().Be("Guy Test");
            }

            CleanupCreatedData("Aa65egw4uywGE3ty");
        }

        private void CleanupCreatedData(string cardId)
        {
            var addedData = _membershipContext.DataCards.Where(x => x.CardId == cardId);
            _membershipContext.DataCards.RemoveRange(addedData);
            _membershipContext.Members.RemoveRange(addedData.Select(x => x.Member));
            _membershipContext.SaveChanges();
        }

        private void PopulateTestData_MemberAndCard(bool isLive)
        {
            var testMember = new DbDataCard
            {
                CardId = "74HytRR87mNJ10pl",
                IsLive = isLive,
                Member = new DbMember
                {
                    EmployeeId = "111",
                    FirstName = "Guy",
                    LastName = "Test",
                    Email = "testguy@email.com",
                    PhoneNumber = "903546380124",
                    SecurityPin = 1466,
                    CompanyId = 1,
                    IsLive = true
                }
            };
            _membershipContext.DataCards.Add(testMember);
            _membershipContext.SaveChanges();
        }

        private void PopulateTestData_Member()
        {
            var testMember = new DbMember
            {
                EmployeeId = "111",
                FirstName = "Guy",
                LastName = "Test",
                Email = "testguy@email.com",
                PhoneNumber = "903546380124",
                SecurityPin = 1466,
                CompanyId = 1,
                IsLive = true
            };
            _membershipContext.Members.Add(testMember);
            _membershipContext.SaveChanges();
        }
    }
}
