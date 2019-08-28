using System;
using System.IO;
using System.IO.Compression;
using System.Text;
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

            var json = IsCompressed ? Unzip(Data) : Data;
            var data = JsonConvert.DeserializeObject<TData>(json, Settings);

            SetupDeserializedData(data);

            return data;
        }

        /// <summary>
        /// Updates properties of deserialized <see cref="Data"/>.
        /// </summary>
        /// <param name="data">Table Entity data.</param>
        protected abstract void SetupDeserializedData(TData data);

        private static string Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }

                return Convert.ToBase64String(mso.ToArray());
            }
        }

        private static string Unzip(string bytes)
        {
            using (var msi = new MemoryStream(Convert.FromBase64String(bytes)))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        private string SerializeData(TData data)
        {
            var json = JsonConvert.SerializeObject(data, Settings);
            return IsCompressed ? Zip(json) : json;
        }
    }
}