using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.EventStore.AzureTable.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Intellias.CQRS.EventStore.AzureTable.Extensions
{
    /// <summary>
    /// Extensions for Event store
    /// </summary>
    public static class EventStoreItemExtensions
    {
        static EventStoreItemExtensions()
        {
            // settings will automatically be used by JsonConvert.SerializeObject/DeserializeObject
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }



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
                Data = JsonConvert.SerializeObject(item),
                EventType = item.GetType().AssemblyQualifiedName,
                ETag = "*"
            };
    }
}