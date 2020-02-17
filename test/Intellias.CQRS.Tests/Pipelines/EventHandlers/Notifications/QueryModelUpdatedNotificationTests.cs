using FluentAssertions;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Signals;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using Intellias.CQRS.Tests.Core.EventHandlers.Tests;
using Intellias.CQRS.Tests.Utils;
using Xunit;

namespace Intellias.CQRS.Tests.Pipelines.EventHandlers.Notifications
{
    public class QueryModelUpdatedNotificationTests
    {
        [Fact]
        public void ConstructorFromMutableQueryModel_Always_PassesCorrelationIdAndAggregateId()
        {
            var integrationEvent = new IntegrationEvent { CorrelationId = Unified.NewCode(), AggregateRootId = Unified.NewCode() };
            var queryModel = new FakeMutableQueryModel { Id = Unified.NewCode() };

            var notification = new QueryModelUpdatedNotification(integrationEvent, queryModel);

            notification.Signal.Should().BeEquivalentTo(new QueryModelUpdatedSignal(queryModel.Id, 0, queryModel.GetType())
            {
                Id = notification.Signal.Id,
                AggregateRootId = integrationEvent.AggregateRootId,
                CorrelationId = integrationEvent.CorrelationId
            });
        }

        [Fact]
        public void ConstructorFromImmutableQueryModel_Always_PassesCorrelationIdAndAggregateId()
        {
            var integrationEvent = new IntegrationEvent { CorrelationId = Unified.NewCode(), AggregateRootId = Unified.NewCode() };
            var queryModel = new FakeImmutableQueryModel { Id = Unified.NewCode(), Version = FixtureUtils.Int() };

            var notification = new QueryModelUpdatedNotification(integrationEvent, queryModel);

            notification.Signal.Should().BeEquivalentTo(new QueryModelUpdatedSignal(queryModel.Id, queryModel.Version, queryModel.GetType())
            {
                Id = notification.Signal.Id,
                AggregateRootId = integrationEvent.AggregateRootId,
                CorrelationId = integrationEvent.CorrelationId
            });
        }
    }
}