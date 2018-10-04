using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Tests.Core.Events
{
    /// <inheritdoc />
    public class TestCreatedEvent : Event
    {
        /// <summary>
        /// TestCreatedEvent
        /// </summary>
        /// <param name="aggregateRootId"></param>
        public TestCreatedEvent(string aggregateRootId)
        {
            AggregateRootId = aggregateRootId;
        }

        /// <summary>
        /// TestData
        /// </summary>
        public string TestData { get; set; }
    }
}
