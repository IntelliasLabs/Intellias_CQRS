using System.Security.Claims;
using AutoFixture;
using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Security;
using Xunit;

namespace Intellias.CQRS.Tests.Core.Security
{
    public class PrincipalTests
    {
        [Fact]
        public void IsInRole_UserHasNoRole_ReturnsFalse()
        {
            var principal = new Principal();

            principal.IsInRole(Unified.NewCode()).Should().BeFalse();
        }

        [Fact]
        public void IsInRole_UserHasAnotherRole_ReturnsFalse()
        {
            var principal = new Principal { Claims = new[] { new IdentityClaim { Type = ClaimTypes.Role, Value = Unified.NewCode() } } };

            principal.IsInRole(Unified.NewCode()).Should().BeFalse();
        }

        [Theory]
        [InlineData("pm")]
        [InlineData("PM")]
        public void IsInRole_UserHasRole_ReturnsTrue(string role)
        {
            var principal = new Principal { Claims = new[] { new IdentityClaim { Type = ClaimTypes.Role, Value = role } } };

            principal.IsInRole(role).Should().BeTrue();
        }

        [Fact]
        public void AsActor_Always_ConvertsCorrectly()
        {
            var principal = new Fixture().Create<Principal>();

            principal.AsActor().Should().BeEquivalentTo(new Actor
            {
                IdentityId = principal.IdentityId,
                UserId = principal.UserId,
                IsProcessManager = principal.IsProcessManager
            });
        }
    }
}