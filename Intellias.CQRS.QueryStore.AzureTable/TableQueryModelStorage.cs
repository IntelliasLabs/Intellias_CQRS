using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Storage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Polly;

namespace Intellias.CQRS.QueryStore.AzureTable
{
    /// <summary>
    /// Azure Table Query Model Storage.
    /// </summary>
    /// <typeparam name="TQueryModel">Query Model Type.</typeparam>
    public class TableQueryModelStorage<TQueryModel> :
        IQueryModelWriter<TQueryModel>,
        IQueryModelReader<TQueryModel>,
        ITableQueryReader<TQueryModel>
        where TQueryModel : class, IQueryModel, new()
    {
        private readonly CloudTable queryTable;
        private readonly CloudTable queryVersioningTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableQueryModelStorage{TQueryModel}"/> class.
        /// </summary>
        /// <param name="account">Azure Storage Account.</param>
        public TableQueryModelStorage(CloudStorageAccount account)
        {
            var client = account.CreateCloudTableClient();

            queryTable = client.GetTableReference(typeof(TQueryModel).Name);
            if (!queryTable.ExistsAsync().GetAwaiter().GetResult())
            {
                queryTable.CreateIfNotExistsAsync().Wait();
            }

            queryVersioningTable = client.GetTableReference($"{typeof(TQueryModel).Name}QueryVersioning");
            if (!queryVersioningTable.ExistsAsync().GetAwaiter().GetResult())
            {
                queryVersioningTable.CreateIfNotExistsAsync().Wait();
            }
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
        public async Task<IReadOnlyCollection<TQueryModel>> GetAllAsync(TableQuery<DynamicTableEntity> query)
        {
            var results = new List<TQueryModel>();
            var continuationToken = new TableContinuationToken();

            do
            {
                var querySegment = await queryTable.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = querySegment.ContinuationToken;

                var queryResults = querySegment.Results.Select(item => DynamicPropertyConverter.ConvertBack<TQueryModel>(item));
                results.AddRange(queryResults);
            }
            while (continuationToken != null);

            return results;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<TQueryModel>> GetAllAsync()
        {
            var query = new TableQuery<DynamicTableEntity>();
            return await GetAllAsync(query);
        }

        /// <inheritdoc />
        public async Task<TQueryModel> GetAsync(string id)
        {
            // Getting entity
            var entity = await RetrieveEntityAsync(id);

            return DynamicPropertyConverter.ConvertBack<TQueryModel>(entity);
        }

#pragma warning disable SCS0005 // Weak random generator
        /// <inheritdoc />
        public async Task UpdateAsync(string id, Action<TQueryModel> updateAction)
        {
            const int numberOfRetries = 5;
            const int coefficientInMiliseconds = 50;
            const int minSaltInMiliseconds = 50;
            const int maxSaltInMiliseconds = 200;

            var jitterer = new Random();

            await Policy
            .Handle<StorageException>()
            .WaitAndRetryAsync(
                numberOfRetries,
                retryAttempt => TimeSpan.FromMilliseconds(coefficientInMiliseconds * Math.Pow(2, retryAttempt))
                    + TimeSpan.FromMilliseconds(jitterer.Next(minSaltInMiliseconds, maxSaltInMiliseconds))) // plus some jitter:)
            .ExecuteAsync(async () =>
            {
                await UpdateActionAsync(id, updateAction);
            });
        }
#pragma warning restore SCS0005 // Weak random generator

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

        private async Task UpdateActionAsync(string id, Action<TQueryModel> updateAction)
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

        private async Task<DynamicTableEntity> RetrieveEntityAsync(string id)
        {
            var readOperation = TableOperation.Retrieve<DynamicTableEntity>(id.Substring(0, 1), id);

            var queryResult = await queryTable.ExecuteAsync(readOperation);

            var entity = (DynamicTableEntity)queryResult.Result;

            if (entity == null)
            {
                throw new KeyNotFoundException(id);
            }

            return entity;
        }
    }
}
