using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core.Commands;
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

            dynamic cmdResult = json.FromJson(sample.GetType());

            Assert.Equal(sample.TestData, cmdResult.TestData);
        }

        [Fact]
        public void MessageExtensionTest()
        {
            var json = sample.ToJson();

            dynamic cmdResult = json.FromJson<IMessage>();

            Assert.Equal(sample.TestData, cmdResult.TestData);
        }
    }
}
