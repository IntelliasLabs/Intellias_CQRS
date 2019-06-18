﻿using FluentAssertions;
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
            var error = "Some error";
            var message = new TestCreatedEvent
            {
                AggregateRootId = "aggregate root",
                CorrelationId = "correlationId",
            };

            var failedEvent = new OperationFailedSignal(message, error);

            failedEvent.Should()
                .Match<OperationFailedSignal>(x => x.CorrelationId == message.CorrelationId).And
                .Match<OperationFailedSignal>(x => x.AggregateRootId == message.AggregateRootId).And
                .Match<OperationFailedSignal>(x => x.Error == error).And
                .Match<OperationFailedSignal>(x => x.Source.Equals(message));
        }
    }
}