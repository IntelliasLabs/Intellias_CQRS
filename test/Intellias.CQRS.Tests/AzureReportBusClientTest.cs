using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Signals;
using Intellias.CQRS.EventBus.AzureServiceBus;
using Intellias.CQRS.EventBus.AzureServiceBus.Extensions;
using Intellias.CQRS.Tests.Core.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Intellias.CQRS.Tests
{
    public class AzureReportBusClientTest
    {
        [Fact]
        public void SubscribeTest()
        {
            var testEvent = new TestCreatedEvent();
            var signal = new OperationCompletedSignal(testEvent);

            var mock = new Mock<ISubscriptionClient>();
            mock.Setup(s => s.RegisterMessageHandler(It.IsAny<Func<Message, CancellationToken, Task>>(), It.IsAny<MessageHandlerOptions>()))
                .Callback<Func<Message, CancellationToken, Task>, MessageHandlerOptions>((handler, _) =>
                 {
                     var msg = CreateBusMessage(signal);
                     handler?.Invoke(msg, CancellationToken.None);
                 });

            var logMock = new Mock<ILogger<AzureReportBusClientV2>>();
            var reportBus = new AzureReportBusClientV2(logMock.Object, mock.Object);

            IMessage? expectedMessage = null;

            reportBus.Subscribe(message =>
            {
                expectedMessage = message;
                return Task.CompletedTask;
            });

            expectedMessage.Should().BeEquivalentTo(signal);
        }

        [Fact]
        public void ServiceBusMessageTest()
        {
            var e = new TestCreatedEvent();
            var msg = e.ToBusMessage();
            var json = Encoding.UTF8.GetString(msg.Body);
            var tms = json.FromJson<IMessage>();

            Assert.Equal(e.Id, tms.Id);
        }

        private static Message CreateBusMessage(IMessage message)
        {
            return new Message(Encoding.UTF8.GetBytes(message.ToJson()))
            {
                MessageId = message.Id,
                ContentType = message.GetType().AssemblyQualifiedName,
                PartitionKey = message.AggregateRootId
            };
        }
    }
}