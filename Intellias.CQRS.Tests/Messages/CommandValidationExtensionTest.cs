using System;
using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Events;
using Xunit;

namespace Intellias.CQRS.Tests.Messages
{
    public class CommandValidationExtensionTest
    {
        [Fact]
        public void ValidateBaseCommandSuccessTest()
        {
            var cmd = new TestCreateCommand
            {
                AggregateRootId = Unified.NewCode(),
                TestData = "some data"
            };
            cmd.Wrap();

            // no exceptions
            cmd.Validate();
        }

        [Fact]
        public void ValidateBaseCommandFailsCommandIdIsNull()
        {
            var cmd = new TestCreateCommand
            {
                TestData = "some data"
            };

            Action act = () => cmd.Validate();
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ValidateBaseCommandFailsAggregateRootIdIsNull()
        {
            var cmd = new TestCreateCommand
            {
                Id = Unified.NewCode(),
                TestData = "some data"
            };

            Action act = () => cmd.Validate();
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ValidateBaseCommandFailsCorrelationIdIsNull()
        {
            var cmd = new TestCreateCommand
            {
                Id = Unified.NewCode(),
                AggregateRootId = Unified.NewCode(),
                TestData = "some data"
            };

            Action act = () => cmd.Validate();
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ValidateBaseCommandFailsRolesIsNull()
        {
            var cmd = new TestCreateCommand
            {
                Id = Unified.NewCode(),
                AggregateRootId = Unified.NewCode(),
                TestData = "some data",
                CorrelationId = Unified.NewCode()
            };

            Action act = () => cmd.Validate();
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ValidateBaseCommandFailUserIdIsNull()
        {
            var cmd = new TestCreateCommand
            {
                Id = Unified.NewCode(),
                AggregateRootId = Unified.NewCode(),
                TestData = "some data",
                CorrelationId = Unified.NewCode()
            };
            cmd.Metadata[MetadataKey.Roles] = "Admin";

            Action act = () => cmd.Validate();
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ValidateBaseCommandFailUserIdIsNotGuid()
        {
            var cmd = new TestCreateCommand
            {
                Id = Unified.NewCode(),
                AggregateRootId = Unified.NewCode(),
                TestData = "some data",
                CorrelationId = Unified.NewCode()
            };
            cmd.Metadata[MetadataKey.Roles] = "Admin";
            cmd.Metadata[MetadataKey.UserId] = Unified.NewCode();

            Action act = () => cmd.Validate();
            act.Should().Throw<FormatException>();
        }

        [Fact]
        public void CommandToEventTest()
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
    }
}
