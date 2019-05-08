using Intellias.CQRS.Core.Config;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core.Commands;
using Newtonsoft.Json;
using Xunit;

namespace Intellias.CQRS.Tests
{
    public class JsonTests
    {
        [Fact]
        public void CommandSerealizationTest()
        {
            var cmd = new TestCreateCommand {
                AggregateRootId = Unified.NewCode(),
                CorrelationId = Unified.NewCode(),
                ExpectedVersion = 3,
                Id = Unified.NewCode(),
                TestData = "test data"
            };

            var json = JsonConvert.SerializeObject(cmd, CqrsSettings.JsonConfig());

            dynamic cmdResult = JsonConvert.DeserializeObject(json, cmd.GetType(), CqrsSettings.JsonConfig());

            Assert.Equal(cmd.TestData, cmdResult.TestData);
        }
    }
}
