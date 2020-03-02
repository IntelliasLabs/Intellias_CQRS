using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Pipelines.EventHandlers;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using Intellias.CQRS.Tests.Core.Fakes;
using Intellias.CQRS.Tests.Utils;
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
        public async Task HandleQueryModelChangedNotification_IsPrivate_DoesntPublishesSignal()
        {
            var integrationEvent = Fixtures.Pipelines.FakeCreatedIntegrationEvent();
            var signal = Fixtures.Pipelines.FakeQueryModelChangedSignal(integrationEvent);
            var notification = new QueryModelChangedNotification(signal)
            {
                IsPrivate = true
            };

            await handler.Handle(notification, CancellationToken.None);

            reportBusStore.Should().BeEmpty();
        }

        [Fact]
        public async Task HandleQueryModelChangedNotification_IsReplay_DoesntPublishesSignal()
        {
            var integrationEvent = Fixtures.Pipelines.FakeCreatedIntegrationEvent();
            var signal = Fixtures.Pipelines.FakeQueryModelChangedSignal(integrationEvent);
            var notification = new QueryModelChangedNotification(signal)
            {
                IsReplay = true
            };

            await handler.Handle(notification, CancellationToken.None);

            reportBusStore.Should().BeEmpty();
        }

        [Fact]
        public async Task HandleQueryModelChangedNotification_IsPublic_PublishesSignal()
        {
            var integrationEvent = Fixtures.Pipelines.FakeCreatedIntegrationEvent();
            var signal = Fixtures.Pipelines.FakeQueryModelChangedSignal(integrationEvent);
            var notification = new QueryModelChangedNotification(signal);

            await handler.Handle(notification, CancellationToken.None);

            reportBusStore.Single().Value.Should().BeEquivalentTo(signal);
        }
    }
}