using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.AzureTable.Core
{
    /// <summary>
    /// TableStoreRecord
    /// </summary>
    public class TableStoreRecord : TableEntity
    {
        /// <summary>
        /// Keeps serialized event itself
        /// </summary>
        public string Data { get; set; }
    }
}
