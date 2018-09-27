using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.EventStore.AzureTable.Documents
{
    /// <summary>
    /// EventStoreItem
    /// </summary>
    public class EventStoreItem : TableEntity
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
