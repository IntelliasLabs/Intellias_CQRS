using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.EventStore.AzureTable.Documents
{
    /// <summary>
    /// StoreEventItem
    /// </summary>
    public class EventStoreEvent : TableEntity
    {

        /// <summary>
        /// Keeps serialized event itself
        /// </summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// Keeps an event type
        /// </summary>
        public string EventType { get; set; } = string.Empty;

        /// <summary>
        /// Version of event
        /// </summary>
        public int Version { get; set; }
    }
}
