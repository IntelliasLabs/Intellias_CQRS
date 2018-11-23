using System;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.QueryStore.AzureTable.Documents;
using Newtonsoft.Json;

namespace Intellias.CQRS.QueryStore.AzureTable.Extensions
{
    /// <summary>
    /// Extensions for ReadModelTableEntity
    /// </summary>
    public static class QueryModelTableEntityExtensions
    {
        /// <summary>
        /// Converts an IReadModel to ReadModelTableEntity
        /// </summary>
        public static QueryModelTableEntity ToStoreEntity(this AbstractQueryModel model) => 
            new QueryModelTableEntity
            {
                PartitionKey = model.GetType().Name,
                RowKey = model.Id,
                Data = JsonConvert.SerializeObject(model),
                Timestamp = DateTime.UtcNow,
                ETag = "*"
            };
    }
}