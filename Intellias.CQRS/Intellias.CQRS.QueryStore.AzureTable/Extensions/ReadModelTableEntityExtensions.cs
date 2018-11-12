using System;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.EventStore.AzureTable.Documents;
using Newtonsoft.Json;

namespace Intellias.CQRS.EventStore.AzureTable.Extensions
{
    /// <summary>
    /// Extensions for ReadModelTableEntity
    /// </summary>
    public static class ReadModelTableEntityExtensions
    {
        /// <summary>
        /// Converts an IReadModel to ReadModelTableEntity
        /// </summary>
        public static ReadModelTableEntity ToStoreEntity(this IReadModel model) => 
            new ReadModelTableEntity
            {
                PartitionKey = model.GetType().Name,
                RowKey = model.Id,
                Data = JsonConvert.SerializeObject(model),
                Timestamp = DateTime.UtcNow,
                ETag = "*"
            };
    }
}