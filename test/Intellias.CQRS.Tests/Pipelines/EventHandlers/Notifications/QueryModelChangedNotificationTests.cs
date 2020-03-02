using FluentAssertions;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using Intellias.CQRS.Tests.Utils;
using Xunit;

namespace Intellias.CQRS.Tests.Pipelines.EventHandlers.Notifications
{
    public class QueryModelChangedNotificationTests
    {
        [Fact]
        public void Constructor_Always_CreatesNotification()
        {
            var isPrivate = FixtureUtils.Bool();
            var integrationEvent = Fixtures.Pipelines.FakeCreatedIntegrationEvent();
            var signal = Fixtures.Pipelines.FakeQueryModelChangedSignal(integrationEvent);

            var notification = new QueryModelChangedNotification(signal) { IsPrivate = isPrivate };

            notification.Signal.Should().BeEquivalentTo(signal);
            notification.IsPrivate.Should().Be(isPrivate);
        }
    }
}