using System;
using System.Linq;
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
            var result = ExecutionResult.Failed("Test error", new Exception("Test exception"));
            result.Error.AddError(new ExecutionError("Name", "Test field error"));

            var json = JsonConvert.SerializeObject(result, CqrsSettings.JsonConfig());

            var deserialized = JsonConvert.DeserializeObject<ExecutionResult>(json, CqrsSettings.JsonConfig());

            Assert.Equal(result.Error.ErrorMessage, deserialized.Error.ErrorMessage);
            Assert.Equal(result.Error.Exception.Message, deserialized.Error.Exception.Message);
            Assert.Equal(result.Success, deserialized.Success);
            Assert.Equal(result.Error.Errors.First().ErrorMessage, deserialized.Error.Errors.First().ErrorMessage);
        }
    }
}
