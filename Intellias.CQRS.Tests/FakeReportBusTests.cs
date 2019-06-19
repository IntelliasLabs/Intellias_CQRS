using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core.Events;
using Intellias.CQRS.Tests.Core.Fakes;
using Xunit;

namespace Intellias.CQRS.Tests
{
    public class FakeReportBusTests
    {
        [Fact]
        public async Task PublishAsyncShoudAddMessageToStore()
        {
            var message = new TestCreatedEvent
            {
                AggregateRootId = "new Id"
            };
            var sut = new FakeReportBus(new Dictionary<string, IMessage>());

            await sut.PublishAsync(message);

            var expectedMessage = await sut.GetMessageAsync(message.AggregateRootId);
            expectedMessage.Should().BeEquivalentTo(message);
        }
    }
}
