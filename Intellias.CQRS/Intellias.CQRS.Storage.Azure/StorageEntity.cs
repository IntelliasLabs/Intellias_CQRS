using Intellias.CQRS.Core.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Globalization;
using System.Linq;

namespace Intellias.CQRS.Storage.Azure
{
    /// <inheritdoc />
    public class StorageEntity : TableEntity
    {
        /// <inheritdoc />
        public StorageEntity()
        {
            ETag = "*";
        }

        /// <inheritdoc />
        public StorageEntity(BaseEntity entity)
        {
            PartitionKey = entity.Id.ToUpperInvariant().First().ToString(CultureInfo.InvariantCulture);
            RowKey = entity.Id.ToUpperInvariant();
            Data = JsonConvert.SerializeObject(entity, Formatting.Indented);
            ETag = "*";
        }

        /// <inheritdoc />
        public string Data { set; get; }

        /// <inheritdoc />
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
