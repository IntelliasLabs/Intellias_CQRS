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
                PartitionKey = "AR",
                LastArVersion = aggregateRoot.Version,
                Timestamp = DateTime.Now,
                ETag = "*"
            };


        /// <summary>
        /// Converts an IEvent to EventStoreEvent
        /// </summary>
        public static EventStoreEvent ToStoreEvent(this IEvent @event) => 
            new EventStoreEvent
            {
                PartitionKey = @event.AggregateRootId,
                RowKey = Unified.NewCode(),
                Data = JsonConvert.SerializeObject(@event),
                EventType = @event.GetType().AssemblyQualifiedName,
                ETag = "*"
            };
    }
}