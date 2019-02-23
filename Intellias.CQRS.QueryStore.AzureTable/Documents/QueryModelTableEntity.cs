using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.QueryStore.AzureTable.Documents
{
    /// <summary>
    /// TableEntity for Read model
    /// </summary>
    public class QueryModelTableEntity : TableEntity
    {

        /// <summary>
        /// Keeps serialized readModel itself
        /// </summary>
        public string Data { get; set; }
    }
}
