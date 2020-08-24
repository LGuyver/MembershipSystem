using Xunit;
using FluentAssertions;
using MembershipSystem.Database;
using System.IdentityModel.Tokens.Jwt;
using System;
using FluentAssertions.Execution;

namespace MembershipSystem.Test
{
    public class UserServiceTests
    {
        private readonly IUserService _userService = new UserService();

        [Fact]
        [Trait("Category", "Unit")]
        public void Authenticate_ShouldReturn_NoApprovedUser()
        {
            const string username = "invalid";
            const string password = "password";

            var response = _userService.Authenticate(username, password);

            response.Should().BeNull();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Authenticate_ShouldReturn_ValidUser()
        {
            const string username = "KIOSK-01";
            const string password = "test";

            var response = _userService.Authenticate(username, password);

            using (new AssertionScope())
            {
                response.FirstName.Should().Be("Test");
                response.LastName.Should().Be("User");
                response.Password.Should().BeNull();
                response.Token.Should().NotBeNullOrEmpty();
            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GenerateToken_ShouldBe_Formatted()
        {
            const string tokenRegex = @"^[A-Za-z0-9-_=]+\.[A-Za-z0-9-_=]+\.?[A-Za-z0-9-_.+/=]*$";
            var response = _userService.GenerateToken(1);

            response.Should().MatchRegex(tokenRegex);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GenerateToken_TokenExpiry_ShouldBe_12Hours()
        {
            var response = _userService.GenerateToken(1);
            var expectedExpiration = DateTime.UtcNow.AddHours(12);

            var token = new JwtSecurityTokenHandler().ReadJwtToken(response);
            var expiration = token.ValidTo;

            expiration.Hour.Should().Be(expectedExpiration.Hour);
        }
    }
}
