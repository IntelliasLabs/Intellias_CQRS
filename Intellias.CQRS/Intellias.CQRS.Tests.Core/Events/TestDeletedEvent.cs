using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Tests.Core.Events
{
    /// <inheritdoc />
    public class TestDeletedEvent : Event
    {
        /// <summary>
        /// TestDeletedEvent
        /// </summary>
        /// <param name="aggregateRootId"></param>
        public TestDeletedEvent(string aggregateRootId)
        {
            AggregateRootId = aggregateRootId;
        }
    }
}
