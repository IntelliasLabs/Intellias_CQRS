using System;
using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core.Commands;
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
    }
}
