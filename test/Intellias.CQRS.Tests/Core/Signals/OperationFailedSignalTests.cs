using AutoFixture;
using FluentAssertions;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;
using Intellias.CQRS.Core.Signals;
using Intellias.CQRS.Tests.Core.Events;
using Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules;
using Newtonsoft.Json;
using Xunit;

namespace Intellias.CQRS.Tests.Core.Signals
{
    public class OperationFailedSignalTests
    {
        private readonly Fixture fixture = new Fixture();

        [Fact]
        public void OperationFailedSignal_Always_Serializable()
        {
            var source = new OperationFailedSignal(fixture.Create<TestCreatedEvent>(), new FailedResult(CoreErrorCodes.ValidationFailed));

            var serialized = JsonConvert.SerializeObject(source);
            var deserialized = JsonConvert.DeserializeObject<OperationFailedSignal>(serialized);

            deserialized.Should().BeEquivalentTo(source);
        }

        [Fact]
        public void OperationFailedSignal_Always_CopiesMessageData()
        {
            var @event = fixture.Create<TestCreatedEvent>();

            var signal = new OperationFailedSignal(@event, new FailedResult(CoreErrorCodes.ValidationFailed));

            signal.Should().BeEquivalentTo(@event, options => options.ForMessage());
        }
    }
}
