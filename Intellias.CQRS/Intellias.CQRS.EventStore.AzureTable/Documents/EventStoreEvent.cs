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
        public string Data { get; set; }

        /// <summary>
        /// Keeps an event type
        /// </summary>
        public string EventType { get; set; }
    }
}
