using System.Linq;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Events;
using Xunit;

namespace Intellias.CQRS.Tests.Messages
{
    public class CommandValidationExtensionTest
    {
        [Fact]
        public void ValidateNullCommandTest()
        {
            var cmd = new TestCreateCommand
            {
                Id = string.Empty
            };

            var result = cmd.Validate();
            Assert.False(result.Success);
        }

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

            var result = cmd.Validate() as ExecutionResult;
            Assert.Equal(nameof(cmd.AggregateRootId), result.Error?.Errors.First().Source);
        }

        [Fact]
        public void ValidateBaseCommandFailsAggregateRootIdIsNull()
        {
            var cmd = new TestCreateCommand
            {
                Id = Unified.NewCode(),
                TestData = "some data"
            };

            var result = cmd.Validate() as ExecutionResult;
            Assert.Equal(nameof(cmd.AggregateRootId), result.Error?.Errors.First().Source);
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

            var result = cmd.Validate() as ExecutionResult;
            Assert.Equal(nameof(cmd.CorrelationId), result.Error?.Errors.First().Source);
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

            var result = cmd.Validate() as ExecutionResult;
            Assert.Equal(nameof(MetadataKey.Roles), result.Error?.Errors.First().Source);
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

            var result = cmd.Validate() as ExecutionResult;
            Assert.Equal(nameof(MetadataKey.UserId), result.Error?.Errors.First().Source);
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

            var result = cmd.Validate() as ExecutionResult;
            Assert.Equal(nameof(MetadataKey.UserId), result.Error?.Errors.First().Source);
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
