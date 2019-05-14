using System.Text;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.EventBus.AzureServiceBus;
using Intellias.CQRS.EventBus.AzureServiceBus.Extensions;
using Intellias.CQRS.Tests.Core.Events;
using Microsoft.Azure.ServiceBus;
using Moq;
using Xunit;

namespace Intellias.CQRS.Tests
{
    public class AzureReportBusClientTest
    {
        [Fact]
        public void SubscribeTest()
        {
            var mock = new Mock<ISubscriptionClient>();

            var reportBus = new AzureReportBusClient(mock.Object);
            reportBus.Subscribe(e => {

                // Is event received
                Assert.NotNull(e);
                return Task.FromResult(e);
            });

            //Todo mock pushing an event
        }

        [Fact]
        public void ServiceBusMessageTest()
        {
            var e = new TestCreatedEvent { Id = "123" };
            var msg = e.ToBusMessage();
            var json = Encoding.UTF8.GetString(msg.Body);
            var tms = json.MessageFromJson();

            Assert.Equal(e.Id, tms.Id);
        }
    }
}
