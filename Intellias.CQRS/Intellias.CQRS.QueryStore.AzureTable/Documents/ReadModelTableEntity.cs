using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.EventStore.AzureTable.Documents
{
    /// <summary>
    /// TableEntity for Read model
    /// </summary>
    public class ReadModelTableEntity : TableEntity
    {

        /// <summary>
        /// Keeps serialized readModel itself
        /// </summary>
        public string Data { get; set; }
    }
}
