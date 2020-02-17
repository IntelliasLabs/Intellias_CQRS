using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Signals;
using Intellias.CQRS.Pipelines.EventHandlers;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using Intellias.CQRS.Tests.Core.EventHandlers.Tests;
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
        public async Task HandleQueryModelUpdatedNotification_IsReplay_DoesntPublishSignal()
        {
            var integrationEvent = new IntegrationEvent { CorrelationId = Unified.NewCode(), AggregateRootId = Unified.NewCode() };
            var queryModel = new FakeMutableQueryModel { Id = Unified.NewCode() };
            var notification = new QueryModelUpdatedNotification(integrationEvent, queryModel)
            {
                IsReplay = true
            };

            await handler.Handle(notification, CancellationToken.None);

            reportBusStore.Should().BeEmpty();
        }

        [Fact]
        public async Task HandleQueryModelUpdatedNotification_IsPrivate_DoesntPublishSignal()
        {
            var integrationEvent = new IntegrationEvent { CorrelationId = Unified.NewCode(), AggregateRootId = Unified.NewCode() };
            var queryModel = new FakeMutableQueryModel { Id = Unified.NewCode() };
            var notification = new QueryModelUpdatedNotification(integrationEvent, queryModel)
            {
                IsPrivate = true
            };

            await handler.Handle(notification, CancellationToken.None);

            reportBusStore.Should().BeEmpty();
        }

        [Fact]
        public async Task HandleQueryModelUpdatedNotification_IsNotReplay_PublishesSignal()
        {
            var integrationEvent = new IntegrationEvent { CorrelationId = Unified.NewCode(), AggregateRootId = Unified.NewCode() };
            var queryModel = new FakeMutableQueryModel { Id = Unified.NewCode() };
            var notification = new QueryModelUpdatedNotification(integrationEvent, queryModel)
            {
                IsReplay = false
            };

            await handler.Handle(notification, CancellationToken.None);

            reportBusStore.Single().Value.Should().BeEquivalentTo(CreateSignal(notification));
        }

        private QueryModelUpdatedSignal CreateSignal(QueryModelUpdatedNotification notification)
        {
            return new QueryModelUpdatedSignal(notification.Signal.QueryModelId, notification.Signal.QueryModelVersion, notification.Signal.QueryModelType)
            {
                Id = notification.Signal.Id,
                AggregateRootId = notification.Signal.AggregateRootId,
                CorrelationId = notification.Signal.CorrelationId
            };
        }
    }
}