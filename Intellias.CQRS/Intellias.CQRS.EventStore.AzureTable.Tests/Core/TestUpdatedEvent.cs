using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.EventStore.AzureTable.Tests.Core
{
    /// <inheritdoc />
    public class TestUpdatedEvent : Event
    {
        /// <summary>
        /// TestUpdateEvent
        /// </summary>
        /// <param name="aggregateRootId"></param>
        public TestUpdatedEvent(string aggregateRootId)
        {
            AggregateRootId = aggregateRootId;
        }

        /// <summary>
        /// TestData
        /// </summary>
        public string TestData { get; set; }
    }
}
