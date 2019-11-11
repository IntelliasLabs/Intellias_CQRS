using System.Linq;
using FluentAssertions;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;
using Xunit;

namespace Intellias.CQRS.Tests.Messages
{
    public class FailedResultTest
    {
        [Fact]
        public void FailedResultSerializeTest()
        {
            var input = new FailedResult(CoreErrorCodes.ValidationFailed, "Some source", "Some message");

            var json = input.ToJson();
            var deserialized = json.FromJson<FailedResult>();

            deserialized.Message.Should().Be(input.Message);
            deserialized.Code.Should().Be(input.Code);
            deserialized.Source.Should().Be(input.Source);
            deserialized.CodeInfo.Should().BeEquivalentTo(input.CodeInfo);
        }

        [Fact]
        public void FailedResultSerializeTestWithDetails()
        {
            var input = new FailedResult(CoreErrorCodes.ValidationFailed, "Some source");
            var executionError1 = new ExecutionError(CoreErrorCodes.NameIsInUse, "Source 1", "Message 1");
            var executionError2 = new ExecutionError(CoreErrorCodes.NameIsNotFound, "Source 2", "Message 2");

            input.AddError(executionError1);
            input.AddError(executionError2);

            var json = input.ToJson();
            var deserialized = json.FromJson<FailedResult>();

            deserialized.Message.Should().Be(input.Message);
            deserialized.Code.Should().Be(input.Code);
            deserialized.Source.Should().Be(input.Source);
            deserialized.CodeInfo.Should().BeEquivalentTo(input.CodeInfo);

            deserialized.Details.Count().Should().Be(2);
            deserialized.Details.First().Should().BeEquivalentTo(executionError1);
            deserialized.Details.Last().Should().BeEquivalentTo(executionError2);
        }
    }
}
