using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Signals;
using Intellias.CQRS.Pipelines.EventHandlers;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using Intellias.CQRS.Tests.Core.Fakes;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Intellias.CQRS.Tests.Pipelines.EventHandlers
{
    public class ExecutionResultNotificationHandlerTests
    {
        private readonly Dictionary<string, IMessage> reportBusStore;
        private readonly ExecutionResultNotificationHandler handler;

        public ExecutionResultNotificationHandlerTests()
        {
            reportBusStore = new Dictionary<string, IMessage>();
            handler = new ExecutionResultNotificationHandler(new FakeReportBus(reportBusStore), NullLogger<ExecutionResultNotificationHandler>.Instance);
        }

        [Fact]
        public async Task HandleQueryModelUpdatedNotification_IsReplay_PublishesSignal()
        {
            var signal = new QueryModelUpdatedSignal("x", 0, typeof(int));
            var notification = new QueryModelUpdatedNotification(signal)
            {
                IsReplay = true
            };

            await handler.Handle(notification, CancellationToken.None);

            reportBusStore.Should().BeEmpty();
        }

        [Fact]
        public async Task HandleQueryModelUpdatedNotification_IsNotReplay_DoesntPublishesSignal()
        {
            var signal = new QueryModelUpdatedSignal("x", 0, typeof(int));
            var notification = new QueryModelUpdatedNotification(signal)
            {
                IsReplay = false
            };

            await handler.Handle(notification, CancellationToken.None);

            reportBusStore.Single().Value.Should().BeEquivalentTo(signal);
        }
    }
}