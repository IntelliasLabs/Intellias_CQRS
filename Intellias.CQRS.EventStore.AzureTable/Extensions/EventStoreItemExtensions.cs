using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.EventStore.AzureTable.Documents;

namespace Intellias.CQRS.EventStore.AzureTable.Extensions
{
    /// <summary>
    /// Extensions for Event store
    /// </summary>
    public static class EventStoreItemExtensions
    {
        /// <summary>
        /// Converts an IEvent to EventStoreEvent
        /// </summary>
        /// <param name="event">event</param>
        public static EventStoreEvent ToStoreEvent(this IEvent @event) =>
            new EventStoreEvent
            {
                PartitionKey = @event.AggregateRootId,
                RowKey = @event.Id,
                Version = @event.Version,
                Data = @event.ToJson(),
                TypeName = @event.TypeName,
                ETag = "*"
            };
    }
}