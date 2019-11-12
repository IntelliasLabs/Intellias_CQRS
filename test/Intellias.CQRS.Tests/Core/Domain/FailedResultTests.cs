using System.Linq;
using FluentAssertions;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;
using Intellias.CQRS.Tests.Utils;
using Xunit;

namespace Intellias.CQRS.Tests.Core.Domain
{
    public class FailedResultTests
    {
        [Fact]
        public void CreateWithInternal_Creates_2LevelError()
        {
            var externalCode = new ErrorCodeInfo(nameof(Core), FixtureUtils.String(), FixtureUtils.String());
            var internalCode = new ErrorCodeInfo(nameof(Core), FixtureUtils.String(), FixtureUtils.String());

            var result = FailedResult.Create(externalCode, internalCode);

            result.Code.Should().Be(externalCode.Code);
            result.Message.Should().Be(externalCode.Message);

            var internalError = result.Details.Single();
            internalError.Message.Should().Be(internalCode.Message);
            internalError.Code.Should().Be(internalCode.Code);
        }

        [Fact]
        public void CreateWithInternal_WithCustomMessage_Returns2LevelError()
        {
            var externalCode = new ErrorCodeInfo(nameof(Core), FixtureUtils.String(), FixtureUtils.String());
            var internalCode = new ErrorCodeInfo(nameof(Core), FixtureUtils.String(), FixtureUtils.String());
            var customMessage = FixtureUtils.String();

            var result = FailedResult.Create(externalCode, internalCode, customMessage);

            result.Code.Should().Be(externalCode.Code);
            result.Message.Should().Be(externalCode.Message);

            var internalError = result.Details.Single();
            internalError.Message.Should().Be(customMessage);
            internalError.Code.Should().Be(internalCode.Code);
        }

        [Fact]
        public void ValidationFailed_WithCollectionOfErrors_ReturnResultWithSameInternalErrors()
        {
            var executionErrors = new[]
            {
                new ExecutionError(new ErrorCodeInfo(nameof(Core), FixtureUtils.String(), FixtureUtils.String())),
                new ExecutionError(new ErrorCodeInfo(nameof(Core), FixtureUtils.String(), FixtureUtils.String()))
            };

            var result = FailedResult.ValidationFailed(executionErrors);

            result.CodeInfo.Should().BeEquivalentTo(CoreErrorCodes.ValidationFailed);
            result.Details.Should().BeEquivalentTo<ExecutionError>(executionErrors);
        }
    }
}
