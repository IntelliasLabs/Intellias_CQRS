using System.Linq;
using FluentAssertions;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;
using Intellias.CQRS.Pipelines.CommandHandlers;
using Intellias.CQRS.Tests.Utils;
using Xunit;

namespace Intellias.CQRS.Tests.Pipelines.CommandHandlers
{
    public class CommandHandlerTests : BaseCommandHandler
    {
        [Fact]
        public void ValidationFailed_WithoutMessage_Test()
        {
            var errorCodeInfo = CoreErrorCodes.UnhandledError;

            var result = ValidationFailed(errorCodeInfo);

            var failedResult = (FailedResult)result;

            failedResult.CodeInfo.Should().Be(CoreErrorCodes.ValidationFailed);
            failedResult.Details.Single().CodeInfo.Should().Be(CoreErrorCodes.UnhandledError);
        }

        [Fact]
        public void ValidationFailed_WithMessage_Test()
        {
            var errorCodeInfo = CoreErrorCodes.UnhandledError;
            var errorMessage = FixtureUtils.String();

            var result = ValidationFailed(errorCodeInfo, errorMessage);

            var failedResult = (FailedResult)result;

            failedResult.CodeInfo.Should().Be(CoreErrorCodes.ValidationFailed);
            failedResult.Details.Single().CodeInfo.Should().Be(CoreErrorCodes.UnhandledError);
            failedResult.Details.Single().Message.Should().Be(errorMessage);
        }

        [Fact]
        public void ValidationFailed_WithErrors_Test()
        {
            var errors = new[]
            {
                new ExecutionError(CoreErrorCodes.UnhandledError)
            };

            var result = ValidationFailed(errors);

            var failedResult = (FailedResult)result;

            failedResult.CodeInfo.Should().Be(CoreErrorCodes.ValidationFailed);
            failedResult.Details.Should().BeEquivalentTo<ExecutionError>(errors);
        }
    }
}
