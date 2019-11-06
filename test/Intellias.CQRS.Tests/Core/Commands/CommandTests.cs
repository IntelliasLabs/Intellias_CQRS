using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;
using Intellias.CQRS.Tests.Core.Events;
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

        [Fact]
        public void Validate_IdIsMissing_Fails()
        {
            var cmd = new FakeCommand().WithValidMetadata();

            cmd.Id = null;

            var result = (FailedResult)cmd.Validate();
            Assert.Equal($"{nameof(FakeCommand)}.{nameof(cmd.Id)}", result.Details.First().Source);
        }

        [Fact]
        public void Validate_AggregateRootIdIsMissing_Fails()
        {
            var cmd = new FakeCommand
            {
                Id = Unified.NewCode()
            };

            var result = (FailedResult)cmd.Validate();
            Assert.Equal($"{nameof(FakeCommand)}.{nameof(cmd.AggregateRootId)}", result.Details.First().Source);
        }

        [Fact]
        public void Validate_CorrelationIdIsMissing_Fails()
        {
            var cmd = new FakeCommand
            {
                Id = Unified.NewCode(),
                AggregateRootId = Unified.NewCode()
            };

            var result = (FailedResult)cmd.Validate();
            Assert.Equal($"{nameof(FakeCommand)}.{nameof(cmd.CorrelationId)}", result.Details.First().Source);
        }

        [Fact]
        public void Validate_RolesAreMissing_Fails()
        {
            var cmd = new FakeCommand
            {
                Id = Unified.NewCode(),
                AggregateRootId = Unified.NewCode(),
                CorrelationId = Unified.NewCode()
            };

            var result = (FailedResult)cmd.Validate();
            Assert.Equal($"{nameof(FakeCommand)}.{nameof(Command.Metadata)}.{nameof(MetadataKey.Roles)}", result.Details.First().Source);
        }

        [Fact]
        public void Validate_UserIdIsMissing_Fails()
        {
            var cmd = new FakeCommand
            {
                Id = Unified.NewCode(),
                AggregateRootId = Unified.NewCode(),
                CorrelationId = Unified.NewCode()
            };
            cmd.Metadata[MetadataKey.Roles] = "Admin";

            var result = (FailedResult)cmd.Validate();
            Assert.Equal($"{nameof(FakeCommand)}.{nameof(Command.Metadata)}.{nameof(MetadataKey.UserId)}", result.Details.First().Source);
        }

        [Fact]
        public void Validate_UserIdIsInvalid_Fails()
        {
            var cmd = new FakeCommand
            {
                Id = Unified.NewCode(),
                AggregateRootId = Unified.NewCode(),
                CorrelationId = Unified.NewCode()
            };
            cmd.Metadata[MetadataKey.Roles] = "Admin";
            cmd.Metadata[MetadataKey.UserId] = Unified.NewCode();

            var result = (FailedResult)cmd.Validate();
            Assert.Equal($"{nameof(FakeCommand)}.{nameof(Command.Metadata)}.{nameof(MetadataKey.UserId)}", result.Details.First().Source);
        }

        [Fact]
        public void ToEvent_Always_CopiesIds()
        {
            var cmd = new TestCreateCommand
            {
                Id = Unified.NewCode(),
                AggregateRootId = Unified.NewCode(),
                TestData = "some data",
                CorrelationId = Unified.NewCode()
            };
            var e = cmd.ToEvent<TestCreatedEvent>();

            Assert.Equal(cmd.AggregateRootId, e.AggregateRootId);
            Assert.Equal(cmd.CorrelationId, e.CorrelationId);
            Assert.Equal(cmd.Id, e.SourceId);
            Assert.Equal(cmd.ExpectedVersion, e.Version);
        }

        private class FakeCommand : Command
        {
            public FakeCommand WithValidMetadata()
            {
                return new FakeCommand
                {
                    Metadata =
                    {
                        { MetadataKey.Roles, "[]" },
                        { MetadataKey.UserId, Guid.NewGuid().ToString() }
                    }
                };
            }
        }
    }
}