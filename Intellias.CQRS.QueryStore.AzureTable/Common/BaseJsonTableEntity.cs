using System;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Intellias.CQRS.QueryStore.AzureTable.Common
{
    /// <summary>
    /// Base abstraction for storing JSON data in <see cref="TableEntity"/>.
    /// </summary>
    /// <typeparam name="TData">Type of data to be stored in Table Entity.</typeparam>
    public abstract class BaseJsonTableEntity<TData> : TableEntity
        where TData : class
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseJsonTableEntity{TData}"/> class.
        /// </summary>
        protected BaseJsonTableEntity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseJsonTableEntity{TData}"/> class.
        /// </summary>
        /// <param name="data">Data to be stored in Table Entity.</param>
        protected BaseJsonTableEntity(TData data)
        {
            Data = JsonConvert.SerializeObject(data, Settings);
        }

        /// <summary>
        /// Serialized data stored in Table Entity.
        /// </summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// Deserializes <see cref="Data"/>.
        /// </summary>
        /// <returns>Deserialized data.</returns>
        public TData DeserializeData()
        {
            if (string.IsNullOrWhiteSpace(Data))
            {
                throw new InvalidOperationException($"Unable to deserialize entity partition key '{PartitionKey}' and row key '{RowKey}' from empty json.");
            }

            return JsonConvert.DeserializeObject<TData>(Data, Settings);
        }

        /// <summary>
        /// Updates properties of deserialized <see cref="Data"/>.
        /// </summary>
        /// <param name="data">Table Entity data.</param>
        protected abstract void SetupDeserializedData(TData data);
    }
}