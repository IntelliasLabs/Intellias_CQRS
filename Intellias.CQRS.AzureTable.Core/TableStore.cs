using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Config;
using Intellias.CQRS.Core.Messages;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Intellias.CQRS.AzureTable.Core
{
    public class TableStore<TEntity>
        where TEntity: class, IIdentified
    {
        private readonly CloudTable table;

        /// <summary>
        /// AzureTableStore
        /// </summary>
        /// <param name="account">Azure Storage Account</param>
        public TableStore(CloudStorageAccount account)
        {
            var client = account.CreateCloudTableClient();
            table = client.GetTableReference(typeof(TEntity).Name);

            // Create the CloudTable if it does not exist
            table.CreateIfNotExistsAsync().Wait();
        }

        /// <summary>
        /// Get entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TEntity> GetAsync(string id)
        {
            var executionResult =
                await table.ExecuteAsync(TableOperation.Retrieve<TableStoreRecord>(id.Substring(0, 1), id));

            var record = (TableStoreRecord)executionResult.Result;
            if (record == null) { throw new KeyNotFoundException(id); }

            return (TEntity)JsonConvert.DeserializeObject(record.Data, CqrsSettings.JsonConfig());
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task UpdateAsync(TEntity entity) =>
            table.ExecuteAsync(TableOperation.Replace(entity.ToTableRecord()));

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task InsertAsync(TEntity entity) =>
            table.ExecuteAsync(TableOperation.Insert(entity.ToTableRecord()));

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task DeleteAsync(TEntity entity) =>
            table.ExecuteAsync(TableOperation.Delete(entity.ToTableRecord()));
    }
}
