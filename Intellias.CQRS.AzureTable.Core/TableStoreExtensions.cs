using System;
using Intellias.CQRS.Core.Config;
using Intellias.CQRS.Core.Messages;
using Newtonsoft.Json;

namespace Intellias.CQRS.AzureTable.Core
{
    public static class TableStoreExtensions
    {
        public static TableStoreRecord ToTableRecord<TEntity>(this TEntity entity)
            where TEntity : class, IIdentified =>
            new TableStoreRecord
            {
                PartitionKey = entity.Id.Substring(0, 1),
                RowKey = entity.Id,
                Data = JsonConvert.SerializeObject(entity, CqrsSettings.JsonConfig()),
                Timestamp = DateTimeOffset.UtcNow,
                ETag = "*"
            };
    }
}
