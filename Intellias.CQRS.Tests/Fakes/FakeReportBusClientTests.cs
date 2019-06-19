using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Tests.Core.Events;
using Intellias.CQRS.Tests.Core.Fakes;
using Xunit;

namespace Intellias.CQRS.Tests.Fakes
{
    public class FakeReportBusClientTests
    {
        [Fact]
        public async Task SubscribeShoudAddMessageToStore()
        {
            var message = new TestCreatedEvent
            {
                AggregateRootId = "new Id"
            };
            var sut = new FakeReportBusClient();

            sut.Subscribe(msg => 
            {
                msg.Should().BeEquivalentTo(message);
                return Task.CompletedTask;
            });

            await sut.PushTestEventAsync(message);
        }
    }
}