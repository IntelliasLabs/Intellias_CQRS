using FluentAssertions;
using Intellias.CQRS.Core.Signals;
using Intellias.CQRS.Tests.Core.Events;
using Xunit;

namespace Intellias.CQRS.Tests.Core.Signals
{
    public class OperationCompletedSignalTests
    {
        [Fact]
        public void OperationCompletedSignalTestshouldCopyPropertiesFromSource()
        {
            var message = new TestCreatedEvent
            {
                AggregateRootId = "aggregate root",
                CorrelationId = "correlationId",
            };

            var failedEvent = new OperationCompletedSignal(message);

            failedEvent.Should()
                .Match<OperationCompletedSignal>(x => x.CorrelationId == message.CorrelationId).And
                .Match<OperationCompletedSignal>(x => x.AggregateRootId == message.AggregateRootId).And
                .Match<OperationCompletedSignal>(x => x.Source.Equals(message));
        }
    }
}
