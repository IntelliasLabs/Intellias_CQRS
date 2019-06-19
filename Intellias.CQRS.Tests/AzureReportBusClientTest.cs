using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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
            var testEvent = new TestCreatedEvent { Id = "id" };

            var mock = new Mock<ISubscriptionClient>();
            mock.Setup(s => s.RegisterMessageHandler(It.IsAny<Func<Message, CancellationToken, Task>>(), It.IsAny<MessageHandlerOptions>()))
                .Callback<Func<Message, CancellationToken, Task>, MessageHandlerOptions>((handler, _) =>
                 {
                     var msg = testEvent.ToBusMessage();
                     handler?.Invoke(msg, CancellationToken.None);
                 });

            var reportBus = new AzureReportBusClient(mock.Object);
            reportBus.Subscribe(message => {

                // Is event received
                message.Should().BeEquivalentTo(testEvent);
                return Task.CompletedTask;
            });
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