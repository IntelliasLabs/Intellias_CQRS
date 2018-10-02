using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.EventStore.AzureTable.Documents
{
    /// <summary>
    /// Aggregate root for storing in Event store.
    /// Used by snapshots, etc.
    /// </summary>
    public class EventStoreAggregate : TableEntity
    {
        /// <summary>
        /// Last version of aggregate root
        /// </summary>
        public int LastArVersion { get; set; }

        /// <summary>
        /// Last version of aggregate root snapshot
        /// </summary>
        public int LastSnapshotVersion { get; set; }
    }
}
