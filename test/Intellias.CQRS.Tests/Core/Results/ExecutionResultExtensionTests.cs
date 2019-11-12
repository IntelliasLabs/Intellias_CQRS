using System.Linq;
using FluentAssertions;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;
using Intellias.CQRS.Core.Results.Extensions;
using Xunit;

namespace Intellias.CQRS.Tests.Core.Results
{
    public class ExecutionResultExtensionTests
    {
        [Fact]
        public void ForCommand_WithSuccessResult_ReturnsSuccess()
        {
            var input = new SuccessfulResult();
            var result = input.ForCommand<TestCommand>(c => c.AggregateRootId);

            result.Should().Be(input);
        }

        [Fact]
        public void ForCommand_NoInnerDetails_ReturnsResultWithCorrectSource()
        {
            var input = new FailedResult(CoreErrorCodes.ValidationFailed);
            var result = input.ForCommand<TestCommand>(c => c.AggregateRootId);

            var failedResult = (FailedResult)result;

            failedResult.Source.Should().Be("TestCommand.AggregateRootId");
            failedResult.Message.Should().Be(input.Message);
            failedResult.Code.Should().Be(input.Code);
            failedResult.CodeInfo.Should().BeEquivalentTo(input.CodeInfo);
        }

        [Fact]
        public void ForCommand_WithInnerDetails_ReturnsResultWithCorrectSource()
        {
            var internalCodeInfo = CoreErrorCodes.AggregateRootNotFound;
            var input = FailedResult.Create(CoreErrorCodes.ValidationFailed, internalCodeInfo);

            var result = input.ForCommand<TestCommand>(c => c.AggregateRootId);

            var failedResult = (FailedResult)result;

            failedResult.Source.Should().Be(nameof(TestCommand));
            failedResult.Message.Should().Be(CoreErrorCodes.ValidationFailed.Message);
            failedResult.Code.Should().Be(CoreErrorCodes.ValidationFailed.Code);
            failedResult.CodeInfo.Should().BeEquivalentTo(CoreErrorCodes.ValidationFailed);

            var internalError = failedResult.Details.Single();

            internalError.Source.Should().Be("TestCommand.AggregateRootId");
            internalError.Message.Should().Be(internalCodeInfo.Message);
            internalError.Code.Should().Be(internalCodeInfo.Code);
            internalError.CodeInfo.Should().BeEquivalentTo(internalCodeInfo);
        }

        private class TestCommand : Command
        {
        }
    }
}
