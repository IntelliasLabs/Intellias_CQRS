using System.Linq;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Tests.Core.Domain;
using Xunit;

namespace Intellias.CQRS.Tests.Messages
{
    public class ExecutionResultTest
    {
        [Fact]
        public void SerializeTest()
        {
            var result = new FailedResult("Test error");
            result.AddError(new ExecutionError("Name", "Test field error"));
            result.AddError(new ExecutionError("Test field error"));
            result.AddError(new ExecutionError(ErrorCodes.ValidationFailed, "Name", "Test field error"));

            var json = result.ToJson();
            var deserialized = json.FromJson<FailedResult>();

            Assert.Equal(result.ErrorMessage, deserialized.ErrorMessage);
            Assert.Equal(result.Success, deserialized.Success);
            Assert.Equal(result.Errors.First().ErrorMessage, deserialized.Errors.First().ErrorMessage);
        }

        [Fact]
        public void AggregateTest()
        {
            var ar = new TestRoot("code");
            Assert.Equal(ErrorCodes.AccessDenied, ((FailedResult)ar.AccessDenied("Test")).ErrorCode);
            Assert.Equal(ErrorCodes.ValidationFailed, ((FailedResult)ar.ValidationFailed("Test")).ErrorCode);
            Assert.Equal(ErrorCodes.UnhandledError, ((FailedResult)ar.UnhandledError("Test")).ErrorCode);
            Assert.Equal(ErrorCodes.VersionConflict, ((FailedResult)ar.Failed(ErrorCodes.VersionConflict, "Test")).ErrorCode);
        }
    }
}
