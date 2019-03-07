using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Storage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.QueryStore.AzureTable
{
    /// <summary>
    /// Azure Table Query Model Storage
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    public class TableQueryModelStorage<TQueryModel> :
        IQueryModelWriter<TQueryModel>,
        IQueryModelReader<TQueryModel>
        where TQueryModel : class, IQueryModel
    {
        private readonly CloudTable queryTable;

        private static DynamicTableEntity Transform(TQueryModel model)
        {
            var entity = new DynamicTableEntity(model.Id.Substring(0, 1), model.Id)
            {
                Properties = DynamicPropertyConverter.Flatten(model),
                Timestamp = DateTime.UtcNow,
                ETag = "*"
            };

            return entity;
        }

        private async Task<DynamicTableEntity> RetrieveEntityAsync(string id)
        {
            var readOperation = TableOperation.Retrieve<DynamicTableEntity>(id.Substring(0, 1), id);

            var queryResult = await queryTable.ExecuteAsync(readOperation);

            var entity = (DynamicTableEntity)queryResult.Result;
            if (entity == null)
            { throw new KeyNotFoundException(id); }

            return entity;
        }

        /// <summary>
        /// TableQueryModelStorage
        /// </summary>
        /// <param name="account">Azure Storage Account</param>
        public TableQueryModelStorage(CloudStorageAccount account)
        {
            var client = account.CreateCloudTableClient();
            queryTable = client.GetTableReference(typeof(TQueryModel).Name);

            // Create the CloudTable if it does not exist
            queryTable.CreateIfNotExistsAsync().Wait();
        }

        /// <inheritdoc />
        public async Task ClearAsync()
        {
            var query = new TableQuery<DynamicTableEntity>();
            var result = await queryTable.ExecuteQuerySegmentedAsync(query, null);

            // Create the batch operation.
            var batchDeleteOperation = new TableBatchOperation();

            foreach (var row in result)
            {
                batchDeleteOperation.Delete(row);
            }

            // Execute the batch operation.
            await queryTable.ExecuteBatchAsync(batchDeleteOperation);
        }

        /// <inheritdoc />
        public async Task CreateAsync(TQueryModel queryModel)
        {
            var operation = TableOperation.Insert(Transform(queryModel));
            await queryTable.ExecuteAsync(operation);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string id)
        {
            var entity = await RetrieveEntityAsync(id);

            // Removing
            var deleteOperation = TableOperation.Delete(entity);
            await queryTable.ExecuteAsync(deleteOperation);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<TQueryModel>> GetAllAsync()
        {
            var query = new TableQuery<DynamicTableEntity>();

            var results = new List<TQueryModel>();
            TableContinuationToken continuationToken = null;

            do
            {
                var querySegment = await queryTable.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = querySegment.ContinuationToken;

                var queryResults = querySegment.Results.Select(item => DynamicPropertyConverter.ConvertBack<TQueryModel>(item.Properties));
                results.AddRange(queryResults);

            } while (continuationToken != null);

            return results;
        }

        /// <inheritdoc />
        public async Task<TQueryModel> GetAsync(string id)
        {
            // Getting entity
            var entity = await RetrieveEntityAsync(id);

            return DynamicPropertyConverter.ConvertBack<TQueryModel>(entity.Properties);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(TQueryModel queryModel)
        {
            // Getting entity
            var entity = await RetrieveEntityAsync(queryModel.Id);

            entity.Properties = DynamicPropertyConverter.Flatten(queryModel);

            entity.Timestamp = DateTime.UtcNow;

            var updateOperation = TableOperation.Replace(entity);
            await queryTable.ExecuteAsync(updateOperation);
        }
    }
}
