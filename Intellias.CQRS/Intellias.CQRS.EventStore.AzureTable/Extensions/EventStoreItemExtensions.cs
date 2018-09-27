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
        /// Converts an IEvent to Event store item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static EventStoreItem ToStoreItem(this IEvent item) => 
            new EventStoreItem
            {
                PartitionKey = item.AggregateRootId,
                RowKey = Unified.NewCode(),
                Data = JsonConvert.SerializeObject(item, Formatting.Indented),
                EventType = item.GetType().AssemblyQualifiedName,
                ETag = "*"
            };
    }
}