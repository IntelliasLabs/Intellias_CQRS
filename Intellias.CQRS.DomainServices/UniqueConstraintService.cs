using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.DomainServices
{
    /// <summary>
    /// UniqueConstraintService
    /// </summary>
    public class UniqueConstraintService : IUniqueConstraintService
    {
        private readonly CloudTable table;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="account"></param>
        public UniqueConstraintService(CloudStorageAccount account)
        {
            var client = account.CreateCloudTableClient();
            table = client.GetTableReference(typeof(UniqueConstraintService).Name);

            // Create the CloudTable if it does not exist
            table.CreateIfNotExistsAsync().Wait();
        }

        /// <summary>
        /// Remove string in store
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="value"></param>
        public async Task RemoveStringAsync(string indexName, string value)
        {
            try
            {
                await table.ExecuteAsync(TableOperation.Delete(new TableEntity
                {
                    PartitionKey = indexName,
                    RowKey = value,
                    Timestamp = DateTimeOffset.UtcNow,
                    ETag = "*"
                }));
            }
            catch (StorageException e)
            {
                throw new InvalidOperationException("Delete operation failed.", e);
            }
        }


        /// <summary>
        /// Reserve string in store
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="value"></param>
        public async Task ReserveStringAsync(string indexName, string value)
        {
            try
            {
                await table.ExecuteAsync(TableOperation.Insert(new TableEntity
                {
                    PartitionKey = indexName,
                    RowKey = value,
                    Timestamp = DateTimeOffset.UtcNow,
                    ETag = "*"
                }));
            }
            catch (StorageException e)
            {
                throw new InvalidOperationException("Reserve operation failed.", e);
            }
        }

        /// <summary>
        /// Update string in store
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public async Task UpdateStringAsync(string indexName, string oldValue, string newValue)
        {
            var updateOperation = new TableBatchOperation();

            updateOperation.Delete(new TableEntity
            {
                PartitionKey = indexName,
                RowKey = oldValue,
                Timestamp = DateTimeOffset.UtcNow,
                ETag = "*"
            });
            updateOperation.Insert(new TableEntity
            {
                PartitionKey = indexName,
                RowKey = newValue,
                Timestamp = DateTimeOffset.UtcNow,
                ETag = "*"
            });

            try
            {
                await table.ExecuteBatchAsync(updateOperation);
            }
            catch (StorageException e)
            {
                throw new InvalidOperationException("Updete operation failed.", e);
            }
        }
    }
}
