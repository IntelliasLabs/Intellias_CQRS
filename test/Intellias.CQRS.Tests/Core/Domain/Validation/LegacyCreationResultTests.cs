using FluentAssertions;
using Intellias.CQRS.Core.Domain.Validation;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Tests.Utils;
using Xunit;

namespace Intellias.CQRS.Tests.Core.Domain.Validation
{
    public class LegacyCreationResultTests
    {
        [Fact]
        public void Constructor_Always_CreatesFailedWithoutEntry()
        {
            var result = new CreationResult<DummyEntry>();

            result.Entry.Should().BeNull();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Succeeded_Always_CreatesSuccessful()
        {
            var entry = new DummyEntry();

            CreationResult.Succeeded(entry).Should().BeEquivalentTo(new CreationResult<DummyEntry>(entry));
        }

        [Fact]
        public void Failed_NotInnerErrors_ReturnsResultWithExecutionError()
        {
            var errorMessage = FixtureUtils.String();
            var expectedError = new ExecutionError(ErrorCodes.ValidationFailed, typeof(DummyEntry).Name, errorMessage);

            CreationResult.Failed<DummyEntry>(errorMessage).Should().BeEquivalentTo(new CreationResult<DummyEntry>(expectedError));
        }

        [Fact]
        public void Failed_WithErrors_ReturnsResultWithInnerExecutionErrors()
        {
            var errorMessage = FixtureUtils.String();
            var innerError = new ExecutionError(ErrorCodes.UnhandledError, FixtureUtils.String(), FixtureUtils.String());

            var expectedError = new ExecutionError(ErrorCodes.ValidationFailed, typeof(DummyEntry).Name, errorMessage);
            var expectedInnerError = new ExecutionError(innerError.Code, $"{typeof(DummyEntry).Name}.{innerError.Source}", innerError.Message);

            CreationResult.Failed<DummyEntry>(errorMessage, innerError).Should()
                .BeEquivalentTo(new CreationResult<DummyEntry>(expectedError, expectedInnerError));
        }

        private class DummyEntry
        {
        }
    }
}