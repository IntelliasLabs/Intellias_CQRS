using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.QueryStore.AzureTable.Options;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.QueryStore.AzureTable.Common
{
    /// <summary>
    /// Base abstraction for Table Storage that provides basic operations.
    /// </summary>
    /// <typeparam name="TTableEntity">Table Entity type.</typeparam>
    public abstract class BaseTableStorage<TTableEntity>
        where TTableEntity : class, ITableEntity, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTableStorage{TTableEntity}"/> class.
        /// </summary>
        /// <param name="options">Value for <see cref="Options"/>.</param>
        /// <param name="tableSuffix">Table name suffix. Along with prefix is used to build table name.</param>
        protected BaseTableStorage(IOptionsMonitor<TableStorageOptions> options, string tableSuffix)
        {
            this.Options = options;
            var client = CloudStorageAccount
                .Parse(options.CurrentValue.ConnectionString)
                .CreateCloudTableClient();

            var tableName = options.CurrentValue.TableNamePrefix + tableSuffix;
            var tableReference = client.GetTableReference(tableName);

            TableProxy = new CloudTableProxy(tableReference, ensureTableExists: true);
        }

        /// <summary>
        /// Table Storage options.
        /// </summary>
        protected IOptionsMonitor<TableStorageOptions> Options { get; }

        /// <summary>
        /// Table proxy.
        /// </summary>
        protected CloudTableProxy TableProxy { get; }

        /// <summary>
        /// Finds entity by keys.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <param name="rowKey">Row key.</param>
        /// <returns>Found entity or NULL.</returns>
        protected async Task<TTableEntity> FindAsync(string partitionKey, string rowKey)
        {
            var operation = TableOperation.Retrieve<TTableEntity>(partitionKey, rowKey);
            var result = await TableProxy.ExecuteAsync(operation);

            return (TTableEntity)result.Result;
        }

        /// <summary>
        /// Inserts entity.
        /// </summary>
        /// <param name="entity">Entity to be inserted.</param>
        /// <returns>Inserted entity.</returns>
        protected async Task<TTableEntity> InsertAsync(TTableEntity entity)
        {
            var operation = TableOperation.Insert(entity);
            var result = await TableProxy.ExecuteAsync(operation);

            return (TTableEntity)result.Result;
        }

        /// <summary>
        /// Replaces entity.
        /// </summary>
        /// <param name="entity">Entity to be replaced.</param>
        /// <returns>Replaced entity.</returns>
        protected async Task<TTableEntity> ReplaceAsync(TTableEntity entity)
        {
            var operation = TableOperation.Replace(entity);
            var result = await TableProxy.ExecuteAsync(operation);

            return (TTableEntity)result.Result;
        }

        /// <summary>
        /// Queries first N entities from partition.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <param name="take">Number of entities to query.</param>
        /// <returns>Found entities.</returns>
        protected async Task<IReadOnlyCollection<TTableEntity>> QueryFirstAsync(string partitionKey, int take)
        {
            var filter = TableQuery.GenerateFilterCondition(nameof(TableEntity.PartitionKey), QueryComparisons.Equal, partitionKey);
            var query = new TableQuery<TTableEntity>()
                .Where(filter)
                .Take(take);

            var querySegment = await TableProxy.ExecuteQuerySegmentedAsync(query, null);
            return querySegment.Results;
        }

        /// <summary>
        /// Queries entities from single partition by row keys.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <param name="rowKeys">Row keys.</param>
        /// <returns>Found entities.</returns>
        protected async Task<IReadOnlyCollection<TTableEntity>> QuerySegmentedAsync(string partitionKey, IReadOnlyCollection<string> rowKeys)
        {
            var idsChunks = rowKeys
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / Options.CurrentValue.QueryChunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();

            var results = new List<TTableEntity>();
            var partitionKeyCondition = TableQuery.GenerateFilterCondition(nameof(TableEntity.PartitionKey), QueryComparisons.Equal, partitionKey);
            foreach (var idsChunk in idsChunks)
            {
                var rowKeyCondition = idsChunk
                    .Select(id => TableQuery.GenerateFilterCondition(nameof(TableEntity.RowKey), QueryComparisons.Equal, id))
                    .Aggregate((current, next) => TableQuery.CombineFilters(current, TableOperators.Or, next));

                var filter = TableQuery.CombineFilters(partitionKeyCondition, TableOperators.And, rowKeyCondition);
                var query = new TableQuery<TTableEntity>().Where(filter);

                results.AddRange(await QueryAllSegmentedAsync(query));
            }

            return results;
        }

        /// <summary>
        /// Queries all entities using query.
        /// </summary>
        /// <param name="query">Query to be executed.</param>
        /// <returns>Found entities.</returns>
        protected async Task<IReadOnlyCollection<TTableEntity>> QueryAllSegmentedAsync(TableQuery<TTableEntity> query)
        {
            var results = new List<TTableEntity>();
            var continuationToken = new TableContinuationToken();

            do
            {
                var querySegment = await TableProxy.ExecuteQuerySegmentedAsync(query, continuationToken);

                results.AddRange(querySegment.Results);

                continuationToken = querySegment.ContinuationToken;
            }
            while (continuationToken != null);

            return results;
        }
    }
}