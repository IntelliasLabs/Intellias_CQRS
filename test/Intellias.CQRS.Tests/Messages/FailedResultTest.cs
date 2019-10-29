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
        public void LegacySerializeTest()
        {
            var result = new FailedResult("Test error");
            result.AddError(new ExecutionError("Name", "Test field error"));
            result.AddError(new ExecutionError("Test field error"));
            result.AddError(new ExecutionError(ErrorCodes.ValidationFailed, "Name", "Test field error"));

            var json = result.ToJson();
            var deserialized = json.FromJson<FailedResult>();

            Assert.Equal(result.Message, deserialized.Message);
            Assert.Equal(result.Success, deserialized.Success);
            Assert.Equal(result.Details.First().Message, deserialized.Details.First().Message);
        }

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
