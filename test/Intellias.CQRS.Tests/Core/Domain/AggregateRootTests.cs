using System.Linq;
using FluentAssertions;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;
using Intellias.CQRS.Tests.Utils;
using Intellias.CQRS.Tests.Utils.Pipelines.Fakes;
using Xunit;

namespace Intellias.CQRS.Tests.Core.Domain
{
    public class AggregateRootTests : AggregateRoot<FakeAggregateState>
    {
        public AggregateRootTests()
            : base(Unified.NewCode())
        {
        }

        [Fact]
        public void AccessDenied_Test()
        {
            var errorCodeInfo = CoreErrorCodes.UnhandledError;

            var failedResult = AccessDenied(errorCodeInfo);

            failedResult.CodeInfo.Should().Be(CoreErrorCodes.AccessDenied);
            failedResult.Details.Single().CodeInfo.Should().Be(CoreErrorCodes.UnhandledError);
        }

        [Fact]
        public void ValidationFailed_WithoutMessage_Test()
        {
            var errorCodeInfo = CoreErrorCodes.UnhandledError;

            var failedResult = ValidationFailed(errorCodeInfo);

            failedResult.CodeInfo.Should().Be(CoreErrorCodes.ValidationFailed);
            failedResult.Details.Single().CodeInfo.Should().Be(CoreErrorCodes.UnhandledError);
        }

        [Fact]
        public void ValidationFailed_WithMessage_Test()
        {
            var errorCodeInfo = CoreErrorCodes.UnhandledError;
            var errorMessage = FixtureUtils.String();

            var failedResult = ValidationFailed(errorCodeInfo, errorMessage);

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

            var failedResult = ValidationFailed(errors);

            failedResult.CodeInfo.Should().Be(CoreErrorCodes.ValidationFailed);
            failedResult.Details.Should().BeEquivalentTo<ExecutionError>(errors);
        }

        [Fact]
        public void LegacyAccessDenied_Test()
        {
            var errorCodeInfo = CoreErrorCodes.UnhandledError;

            var failedResult = AccessDeniedWithCode(errorCodeInfo);

            failedResult.CodeInfo.Should().Be(CoreErrorCodes.AccessDenied);
            failedResult.Details.Single().CodeInfo.Should().Be(CoreErrorCodes.UnhandledError);
        }

        [Fact]
        public void LegacyValidationFailed_WithoutMessage_Test()
        {
            var errorCodeInfo = CoreErrorCodes.UnhandledError;

            var failedResult = ValidationFailedWithCode(errorCodeInfo);

            failedResult.CodeInfo.Should().Be(CoreErrorCodes.ValidationFailed);
            failedResult.Details.Single().CodeInfo.Should().Be(CoreErrorCodes.UnhandledError);
        }

        [Fact]
        public void LegacyValidationFailed_WithMessage_Test()
        {
            var errorCodeInfo = CoreErrorCodes.UnhandledError;
            var errorMessage = FixtureUtils.String();

            var failedResult = ValidationFailedWithCode(errorCodeInfo, errorMessage);

            failedResult.CodeInfo.Should().Be(CoreErrorCodes.ValidationFailed);
            failedResult.Details.Single().CodeInfo.Should().Be(CoreErrorCodes.UnhandledError);
            failedResult.Details.Single().Message.Should().Be(errorMessage);
        }

        [Fact]
        public void LegacyValidationFailed_WithErrors_Test()
        {
            var errors = new[]
            {
                new ExecutionError(CoreErrorCodes.UnhandledError)
            };

            var failedResult = ValidationFailedWithDetails(errors);

            failedResult.CodeInfo.Should().Be(CoreErrorCodes.ValidationFailed);
            failedResult.Details.Should().BeEquivalentTo<ExecutionError>(errors);
        }
    }
}
