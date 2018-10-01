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
            Data = JsonConvert.SerializeObject(entity);
            ETag = "*";
        }

        /// <summary>
        /// Holds the entity data
        /// </summary>
        public string Data { set; get; }

        /// <summary>
        /// Returns deserialized data object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetValue<T>() => 
            string.IsNullOrEmpty(Data) 
                ? default(T) 
                : JsonConvert.DeserializeObject<T>(Data);
        
    }
}
