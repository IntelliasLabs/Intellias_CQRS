using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Config;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core.Commands;
using Newtonsoft.Json;
using Xunit;

namespace Intellias.CQRS.Tests
{
    public class JsonTests
    {
        private readonly TestCreateCommand sample = new TestCreateCommand
        {
            AggregateRootId = Unified.NewCode(),
            CorrelationId = Unified.NewCode(),
            ExpectedVersion = 3,
            Id = Unified.NewCode(),
            TestData = "test data"
        };

        [Fact]
        public void CommandSerealizationTest()
        {
            var json = sample.ToJson();

            dynamic cmdResult = JsonConvert.DeserializeObject(json, sample.GetType(), CqrsSettings.JsonConfig());

            Assert.Equal(sample.TestData, cmdResult.TestData);
        }

        [Fact]
        public void MessageExtensionTest()
        {
            var json = sample.ToJson();

            dynamic cmdResult = json.MessageFromJson();

            Assert.Equal(sample.TestData, cmdResult.TestData);
        }
    }
}
