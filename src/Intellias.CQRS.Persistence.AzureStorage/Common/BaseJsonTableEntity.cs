using System;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace Intellias.CQRS.Persistence.AzureStorage.Common
{
    /// <summary>
    /// Base abstraction for storing JSON data in <see cref="TableEntity"/>.
    /// </summary>
    /// <typeparam name="TData">Type of data to be stored in Table Entity.</typeparam>
    public abstract class BaseJsonTableEntity<TData> : TableEntity
        where TData : class
    {
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
        /// <param name="isCompressed">Is GZip compression is enabled.</param>
        protected BaseJsonTableEntity(TData data, bool isCompressed)
        {
            IsCompressed = isCompressed;
            Data = SerializeData(data);
        }

        /// <summary>
        /// Serialized data stored in Table Entity.
        /// </summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// Compresses JSON <see cref="Data"/> using GZip if True.
        /// </summary>
        public bool IsCompressed { get; set; }

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

            var json = IsCompressed ? Data.Unzip() : Data;
            var data = JsonConvert.DeserializeObject<TData>(json, TableStorageJsonSerializerSettings.GetDefault());

            SetupDeserializedData(data);

            return data;
        }

        /// <summary>
        /// Updates properties of deserialized <see cref="Data"/>.
        /// </summary>
        /// <param name="data">Table Entity data.</param>
        protected abstract void SetupDeserializedData(TData data);

        private string SerializeData(TData data)
        {
            var json = JsonConvert.SerializeObject(data, TableStorageJsonSerializerSettings.GetDefault());
            return IsCompressed ? json.Zip() : json;
        }
    }
}