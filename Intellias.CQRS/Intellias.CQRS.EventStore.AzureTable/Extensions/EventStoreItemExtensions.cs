using System;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.EventStore.AzureTable.Documents;
using Newtonsoft.Json;

namespace Intellias.CQRS.EventStore.AzureTable.Extensions
{
    /// <summary>
    /// Extensions for Event store
    /// </summary>
    public static class EventStoreItemExtensions
    {
        /// <summary>
        /// Converts AggregateRoot to EventStoreAggregate
        /// </summary>
        /// <param name="aggregateRoot"></param>
        /// <returns></returns>
        public static EventStoreAggregate ToStoreAggregate(this IAggregateRoot aggregateRoot) =>
            new EventStoreAggregate
            {
                RowKey = aggregateRoot.Id,
                // Use type of agregate as partition key for optimal fetching
                PartitionKey = aggregateRoot.GetType().Name,
                // Most of aggregates do have parent-child relations, so keeping parentId here for unification
                ParentId = aggregateRoot is IChildEntity 
                    ? ((IChildEntity) aggregateRoot).ParentId 
                    : Unified.Dummy,
                LastArVersion = aggregateRoot.Version,
                Timestamp = DateTime.UtcNow,
                ETag = "*"
            };

        /// <summary>
        /// Converts an IEvent to EventStoreEvent
        /// </summary>
        public static EventStoreEvent ToStoreEvent(this IEvent @event) => 
            new EventStoreEvent
            {
                PartitionKey = @event.AggregateRootId,
                RowKey = @event.Id,
                Version = @event.Version,
                Data = JsonConvert.SerializeObject(@event),
                EventType = @event.GetType().Name,
                ETag = "*"
            };
    }
}