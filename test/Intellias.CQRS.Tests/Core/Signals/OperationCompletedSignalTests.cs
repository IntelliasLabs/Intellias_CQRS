using AutoFixture;
using FluentAssertions;
using Intellias.CQRS.Core.Signals;
using Intellias.CQRS.Tests.Core.Events;
using Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules;
using Newtonsoft.Json;
using Xunit;

namespace Intellias.CQRS.Tests.Core.Signals
{
    public class OperationCompletedSignalTests
    {
        private readonly Fixture fixture = new Fixture();

        [Fact]
        public void OperationCompletedSignal_Always_Serializable()
        {
            var source = new OperationCompletedSignal(fixture.Create<TestCreatedEvent>());

            var serialized = JsonConvert.SerializeObject(source);
            var deserialized = JsonConvert.DeserializeObject<OperationCompletedSignal>(serialized);

            deserialized.Should().BeEquivalentTo(source);
        }

        [Fact]
        public void OperationCompletedSignal_Always_CopiesMessageData()
        {
            var @event = fixture.Create<TestCreatedEvent>();

            var signal = new OperationCompletedSignal(@event);

            signal.Should().BeEquivalentTo(@event, options => options.ForMessage());
        }
    }
}
