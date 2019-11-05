using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Results.Errors;
using Xunit;

namespace Intellias.CQRS.Tests.Core.Commands
{
    public class CommandTests
    {
        [Fact]
        public void Command_RolesAreMissing_FailsValidation()
        {
            var command = new FakeCommand
            {
                Metadata = { [MetadataKey.UserId] = Guid.NewGuid().ToString() }
            };

            var error = command.Validate(new ValidationContext(command)).Single();
            error.Code.Should().Be(CoreErrorCodes.NotFound.Code);
            error.Source.Should().Be($"{nameof(Command.Metadata)}.{MetadataKey.Roles}");
        }

        [Fact]
        public void Command_UserIdIsMissing_FailsValidation()
        {
            var command = new FakeCommand
            {
                Metadata = { [MetadataKey.Roles] = "roles" }
            };

            var error = command.Validate(new ValidationContext(command)).Single();
            error.Code.Should().Be(CoreErrorCodes.NotFound.Code);
            error.Source.Should().Be($"{nameof(Command.Metadata)}.{MetadataKey.UserId}");
        }

        [Fact]
        public void Command_UserIdIsInvalid_FailsValidation()
        {
            var command = new FakeCommand
            {
                Metadata =
                {
                    [MetadataKey.Roles] = "roles",
                    [MetadataKey.UserId] = "invalid guid"
                }
            };

            var error = command.Validate(new ValidationContext(command)).Single();
            error.Code.Should().Be(CoreErrorCodes.InvalidFormat.Code);
            error.Source.Should().Be($"{nameof(Command.Metadata)}.{MetadataKey.UserId}");
        }

        private class FakeCommand : Command
        {
        }
    }
}