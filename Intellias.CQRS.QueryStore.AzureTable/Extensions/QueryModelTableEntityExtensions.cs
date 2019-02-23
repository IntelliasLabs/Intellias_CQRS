using System;
using Intellias.CQRS.Core.Config;
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
        public static QueryModelTableEntity ToStoreEntity(this IQueryModel model) => 
            new QueryModelTableEntity
            {
                PartitionKey = model.ParentId,
                RowKey = model.Id,
                Data = JsonConvert.SerializeObject(model, CqrsSettings.JsonConfig()),
                Timestamp = DateTime.UtcNow,
                ETag = "*"
            };
    }
}