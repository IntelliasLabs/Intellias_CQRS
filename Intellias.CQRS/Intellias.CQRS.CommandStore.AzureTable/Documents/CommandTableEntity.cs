using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.CommandStore.AzureTable.Documents
{
    /// <summary>
    /// CommandTableEntity
    /// </summary>
    public class CommandTableEntity : TableEntity
    {
        /// <summary>
        /// Keeps serialized event itself
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Keeps an event type
        /// </summary>
        public string CommandType { get; set; }

        /// <summary>
        /// Version of command
        /// </summary>
        public int Version { get; set; }
    }
}
