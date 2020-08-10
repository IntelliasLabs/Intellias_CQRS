using System.Text;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.EventBus.AzureServiceBus.Extensions;
using Intellias.CQRS.Tests.Core.Commands;
using Microsoft.Azure.ServiceBus;
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

            dynamic cmdResult = json.FromJson<TestCreateCommand>();

            Assert.Equal(sample.TestData, cmdResult.TestData);
        }

        [Fact]
        public void MessageExtensionServiceBusTest()
        {
            var json = sample.ToJson();

            var msg = new Message(Encoding.UTF8.GetBytes(json))
            {
                MessageId = sample.Id,
                ContentType = sample.GetType().AssemblyQualifiedName,
                PartitionKey = AbstractMessage.GlobalSessionId,
                CorrelationId = sample.CorrelationId,
                SessionId = AbstractMessage.GlobalSessionId,
                Label = sample.GetType().Name
            };

            var cmd = msg.GetCommand();

            Assert.Equal(sample.AggregateRootId, cmd.AggregateRootId);
        }
    }
}
