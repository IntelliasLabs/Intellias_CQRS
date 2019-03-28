using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Storage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.QueryStore.AzureTable
{
    /// <summary>
    /// Azure Table Query Model Storage
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    public class TableQueryModelStorage<TQueryModel> :
        IQueryModelWriter<TQueryModel>,
        IQueryModelReader<TQueryModel>
        where TQueryModel : class, IQueryModel, new()
    {
        private readonly CloudTable queryTable;
        private readonly CloudTable queryVersioningTable;

        private static DynamicTableEntity Transform(TQueryModel model)
        {
            var entity = new DynamicTableEntity(model.Id.Substring(0, 1), model.Id)
            {
                Properties = DynamicPropertyConverter.Flatten(model),
                Timestamp = DateTimeOffset.UtcNow,
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
            queryVersioningTable = client.GetTableReference($"{typeof(TQueryModel).Name}QueryVersioning");

            // Create the CloudTable if it does not exist
            queryTable.CreateIfNotExistsAsync().Wait();
            queryVersioningTable.CreateIfNotExistsAsync().Wait();
        }

        /// <inheritdoc />
        public async Task ReserveEventAsync(IEvent @event)
        {
            var entity = new DynamicTableEntity(@event.AggregateRootId, @event.Id);
            var op = TableOperation.Insert(entity);
            await queryVersioningTable.ExecuteAsync(op); // Will fail for second event if it was dublicated
        }

        /// <inheritdoc />
        public async Task ClearAsync()
        {
            var query = new TableQuery<DynamicTableEntity>();
            var result = await queryTable.ExecuteQuerySegmentedAsync(query, null);

            foreach (var entity in result)
            {
                var operation = TableOperation.Delete(entity);
                await queryTable.ExecuteAsync(operation);
            }
        }

        /// <inheritdoc />
        public async Task CreateAsync(TQueryModel queryModel)
        {
            var entity = Transform(queryModel);
            var operation = TableOperation.Insert(entity);
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
            var continuationToken = new TableContinuationToken();

            do
            {
                var querySegment = await queryTable.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = querySegment.ContinuationToken;

                var queryResults = querySegment.Results.Select(item => DynamicPropertyConverter.ConvertBack<TQueryModel>(item));
                results.AddRange(queryResults);

            } while (continuationToken != null);

            return results;
        }

        /// <inheritdoc />
        public async Task<TQueryModel> GetAsync(string id)
        {
            // Getting entity
            var entity = await RetrieveEntityAsync(id);

            return DynamicPropertyConverter.ConvertBack<TQueryModel>(entity);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(string id, Action<TQueryModel> updateAction)
        {
            // Getting entity
            var entity = await RetrieveEntityAsync(id);

            var queryModel = DynamicPropertyConverter.ConvertBack<TQueryModel>(entity);

            // calling update
            updateAction?.Invoke(queryModel);

            entity.Properties = DynamicPropertyConverter.Flatten(queryModel);

            var updateOperation = TableOperation.Replace(entity);
            await queryTable.ExecuteAsync(updateOperation);
        }
    }
}
