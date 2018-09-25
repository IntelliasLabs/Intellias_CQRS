using Intellias.CQRS.Core.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intellias.CQRS.Storage.Azure
{
    public class StorageEntity : TableEntity
    {
        public StorageEntity()
        {
            ETag = "*";
        }

        public StorageEntity(BaseEntity entity)
        {
            PartitionKey = entity.Id.ToUpperInvariant().First().ToString();
            RowKey = entity.Id.ToUpperInvariant();
            Data = JsonConvert.SerializeObject(entity, Formatting.Indented);
            ETag = "*";
        }

        public string Data { set; get; }

        public T GetValue<T>()
        {
            if (string.IsNullOrEmpty(Data))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(Data);
        }
    }
}
