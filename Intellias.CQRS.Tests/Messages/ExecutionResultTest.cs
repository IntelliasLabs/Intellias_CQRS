using System;
using System.Linq;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Config;
using Intellias.CQRS.Core.Results;
using Newtonsoft.Json;
using Xunit;

namespace Intellias.CQRS.Tests.Messages
{
    public class ExecutionResultTest
    {
        [Fact]
        public void SerializeTest()
        {
            var result = new FailedResult("Test error", new Exception("Test exception"));
            result.AddError(new ExecutionError("Name", "Test field error"));
            result.AddError(new ExecutionError("Test field error"));
            result.AddError(new ExecutionError(ErrorCodes.ValidationFailed, "Name", "Test field error"));

            var json = result.ToJson();
            var deserialized = json.FromJson<FailedResult>();

            Assert.Equal(result.ErrorMessage, deserialized.ErrorMessage);
            Assert.Equal(result.Exception?.Message, deserialized.Exception?.Message);
            Assert.Equal(result.Success, deserialized.Success);
            Assert.Equal(result.Errors.First().ErrorMessage, deserialized.Errors.First().ErrorMessage);
        }
    }
}
