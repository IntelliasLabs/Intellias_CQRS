using FluentAssertions;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Signals;
using Intellias.CQRS.Tests.Core.Events;
using Xunit;

namespace Intellias.CQRS.Tests.Core.Signals
{
    public class OperationFailedSignalTests
    {
        [Fact]
        public void OperationFailedSignalShouldCopyPropertiesFromSource()
        {
            const string error = "Some error";
            var message = new TestCreatedEvent
            {
                AggregateRootId = "aggregate root",
                CorrelationId = "correlationId",
            };

            var failedEvent = new OperationFailedSignal(message, new FailedResult(error));
            failedEvent = (OperationFailedSignal)failedEvent.ToJson().MessageFromJson();

            failedEvent.Should()
                .Match<OperationFailedSignal>(x => x.CorrelationId == message.CorrelationId).And
                .Match<OperationFailedSignal>(x => x.AggregateRootId == message.AggregateRootId).And
                .Match<OperationFailedSignal>(x => x.Error.ErrorMessage == error).And
                .Match<OperationFailedSignal>(x => x.Source.Equals(message));
        }
    }
}
