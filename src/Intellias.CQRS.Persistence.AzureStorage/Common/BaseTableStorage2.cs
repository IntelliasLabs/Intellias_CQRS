using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Tables.SharedFiles;

namespace Intellias.CQRS.Persistence.AzureStorage.Common
{
    /// <summary>
    /// Base abstraction for Table Storage that provides basic operations.
    /// </summary>
    public abstract class BaseTableStorage2
    {
        /// <summary>
        /// Transaction can include at most 100 entities.
        /// From docs https://docs.microsoft.com/en-us/rest/api/storageservices/performing-entity-group-transactions.
        /// </summary>
        private const byte BatchOperationMaxSize = 100;

        /// <summary>
        /// Empirically chosen value of maximum number of partition/row keys comparisons in a single query.
        /// Is limited by the maximum recursion depth of generated OData query or the size of generated URI.
        /// It's taken from worse case measurement when all items have unique partition key.
        /// </summary>
        private const ushort QueryFilterArgumentsMaxSize = 110;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTableStorage2"/> class.
        /// </summary>
        /// <param name="options">Value for <see cref="Options"/>.</param>
        /// <param name="tableName">Table name suffix. Along with prefix is used to build table name.</param>
        protected BaseTableStorage2(ITableStorageOptions options, string tableName)
        {
            this.Options = options;
            var client = CloudStorageAccount
                .Parse(options.ConnectionString)
                .CreateCloudTableClient();

            var tableFullName = options.TableNamePrefix + tableName;
            var tableReference = client.GetTableReference(tableFullName);

            TableProxy = new CloudTableProxy(tableReference, ensureTableExists: true);
        }

        /// <summary>
        /// Table Storage options.
        /// </summary>
        protected ITableStorageOptions Options { get; }

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
        protected async Task<DynamicTableEntity> FindAsync(string partitionKey, string rowKey)
        {
            var operation = TableOperation.Retrieve(partitionKey, rowKey);
            var result = await TableProxy.ExecuteAsync(operation);

            return (DynamicTableEntity)result.Result;
        }

        /// <summary>
        /// Inserts entity.
        /// </summary>
        /// <param name="entity">Entity to be inserted.</param>
        /// <returns>Inserted entity.</returns>
        protected async Task<DynamicTableEntity> InsertAsync(DynamicTableEntity entity)
        {
            var operation = TableOperation.Insert(entity);
            var result = await TableProxy.ExecuteAsync(operation);

            return (DynamicTableEntity)result.Result;
        }

        /// <summary>
        /// Replaces entity.
        /// </summary>
        /// <param name="entity">Entity to be replaced.</param>
        /// <returns>Replaced entity.</returns>
        protected async Task<DynamicTableEntity> ReplaceAsync(DynamicTableEntity entity)
        {
            var operation = TableOperation.Replace(entity);
            var result = await TableProxy.ExecuteAsync(operation);

            return (DynamicTableEntity)result.Result;
        }

        /// <summary>
        /// Merges entity.
        /// </summary>
        /// <param name="entity">Entity to be merged with existing.</param>
        /// <returns>Merged entity.</returns>
        protected async Task<DynamicTableEntity> MergeAsync(DynamicTableEntity entity)
        {
            var operation = TableOperation.Merge(entity);
            var result = await TableProxy.ExecuteAsync(operation);

            return (DynamicTableEntity)result.Result;
        }

        /// <summary>
        /// Inserts or replaces entity.
        /// </summary>
        /// <param name="entity">Entity to be inserted or replaced.</param>
        /// <returns>Inserted or replaced entity.</returns>
        protected async Task<DynamicTableEntity> InsertOrReplaceAsync(DynamicTableEntity entity)
        {
            var operation = TableOperation.InsertOrReplace(entity);
            var result = await TableProxy.ExecuteAsync(operation);

            return (DynamicTableEntity)result.Result;
        }

        /// <summary>
        /// Deletes entity. Idempotent.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <param name="rowKey">Row key.</param>
        /// <returns>Deleted entity.</returns>
        protected async Task DeleteAsync(string partitionKey, string rowKey)
        {
            var operation = TableOperation.Delete(new DynamicTableEntity(partitionKey, rowKey, "*", new Dictionary<string, EntityProperty>()));

            try
            {
                await TableProxy.ExecuteAsync(operation);
            }
            catch (StorageException exception)
                when (exception.RequestInformation?.ExtendedErrorInformation?.ErrorCode == StorageErrorCodeStrings.ResourceNotFound)
            {
                // Swallow ResourceNotFound response to support idempotence in delete operation.
            }
        }

        /// <summary>
        /// Deletes all entities in the partition. Idempotent.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected async Task DeleteAllAsync(string partitionKey)
        {
            var queryAllEntities = new TableQuery<DynamicTableEntity>()
                .Where(TableQuery.GenerateFilterCondition(nameof(TableEntity.PartitionKey), QueryComparisons.Equal, partitionKey))
                .Select(new[] { nameof(TableEntity.PartitionKey), nameof(TableEntity.RowKey) });

            var allEntities = await QueryAllSegmentedAsync(queryAllEntities);

            await ExecuteBatchAsync(allEntities, (te, bo) => bo.Delete(te), async bo =>
            {
                try
                {
                    await TableProxy.ExecuteBatchAsync(bo);
                }
                catch (StorageException exception)
                    when (exception.RequestInformation?.ExtendedErrorInformation?.ErrorCode == StorageErrorCodeStrings.ResourceNotFound)
                {
                    // Swallow ResourceNotFound response to support idempotence in delete operation.
                }
            });
        }

        /// <summary>
        /// Queries first N entities from partition.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <param name="take">Number of entities to query.</param>
        /// <returns>Found entities.</returns>
        protected async Task<IReadOnlyCollection<DynamicTableEntity>> QueryFirstAsync(string partitionKey, int take)
        {
            var filter = TableQuery.GenerateFilterCondition(nameof(TableEntity.PartitionKey), QueryComparisons.Equal, partitionKey);
            var query = new TableQuery<DynamicTableEntity>()
                .Where(filter)
                .Take(take);

            var querySegment = await TableProxy.ExecuteQuerySegmentedAsync(query, null);
            return querySegment.Results;
        }

        /// <summary>
        /// Queries all entities using query.
        /// </summary>
        /// <param name="query">Query to be executed.</param>
        /// <returns>Found entities.</returns>
        protected async Task<IReadOnlyCollection<DynamicTableEntity>> QueryAllSegmentedAsync(TableQuery<DynamicTableEntity> query)
        {
            var results = new List<DynamicTableEntity>();
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

        /// <summary>
        /// Queries all entities using multiple queries.
        /// </summary>
        /// <param name="queries">Queries to be executed.</param>
        /// <returns>Found entities.</returns>
        protected async Task<IReadOnlyCollection<DynamicTableEntity>> QueryAllSegmentedAsync(IEnumerable<TableQuery<DynamicTableEntity>> queries)
        {
            var results = new List<DynamicTableEntity>();
            foreach (var query in queries)
            {
                var queryResults = await QueryAllSegmentedAsync(query);

                results.AddRange(queryResults);
            }

            return results;
        }

        /// <summary>
        /// Executes batch operation. Considers max size of a batch operation.
        /// </summary>
        /// <param name="entities">Entities to be part of a batch.</param>
        /// <param name="updateBatch">Update batch with entity.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected Task ExecuteBatchAsync(IReadOnlyCollection<ITableEntity> entities, Action<ITableEntity, TableBatchOperation> updateBatch)
        {
            return ExecuteBatchAsync(entities, updateBatch, b => TableProxy.ExecuteBatchAsync(b));
        }

        /// <summary>
        /// Executes batch operation. Considers max size of a batch operation.
        /// </summary>
        /// <param name="entities">Entities to be part of a batch.</param>
        /// <param name="updateBatch">Update batch with entity.</param>
        /// <param name="executeBatch">Executes batch operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected async Task ExecuteBatchAsync(IReadOnlyCollection<ITableEntity> entities, Action<ITableEntity, TableBatchOperation> updateBatch, Func<TableBatchOperation, Task> executeBatch)
        {
            if (entities.Count == 0)
            {
                return;
            }

            var chunks = entities
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / BatchOperationMaxSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToArray();

            foreach (var chunk in chunks)
            {
                var batchOperation = new TableBatchOperation();
                foreach (var entity in chunk)
                {
                    updateBatch(entity, batchOperation);
                }

                await executeBatch(batchOperation);
            }
        }

        /// <summary>
        /// Creates table queries from collection of partition and row keys.
        /// If sequence of <paramref name="keys"/> is large, splits it to batches and creates query per batch.
        /// </summary>
        /// <param name="keys">Pairs of partition and row keys.</param>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <returns>Created queries.</returns>
        protected IEnumerable<TableQuery<TEntity>> CreateQueryByKeys<TEntity>(IReadOnlyCollection<(string PartitionKey, string RowKey)> keys)
        {
            // Split all keys into batches to fit the table query limits.
            var queryKeysChunks = keys
                .OrderBy(key => key.PartitionKey)
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / QueryFilterArgumentsMaxSize)
                .Select(x => x
                    .Select(v => v.Value)
                    .Select(key => new { key.PartitionKey, key.RowKey })
                    .GroupBy(k => k.PartitionKey, k => k.RowKey))
                .ToArray();

            // Create query for each keys batch.
            foreach (var queryKeys in queryKeysChunks)
            {
                var partitionFilters = new List<string>();
                foreach (var partitionKey in queryKeys)
                {
                    var partitionKeyCondition = TableQuery.GenerateFilterCondition(nameof(TableEntity.PartitionKey), QueryComparisons.Equal, partitionKey.Key);
                    var rowKeyConditionSeq = partitionKey
                        .Select(rk => "(" + TableQuery.GenerateFilterCondition(nameof(TableEntity.RowKey), QueryComparisons.Equal, rk) + ")");

                    var rowKeyCondition = string.Join($" {TableOperators.Or} ", rowKeyConditionSeq);
                    var partitionFilter = TableQuery.CombineFilters(partitionKeyCondition, TableOperators.And, rowKeyCondition);

                    partitionFilters.Add(partitionFilter);
                }

                var filter = partitionFilters.Aggregate((current, next) => TableQuery.CombineFilters(current, TableOperators.Or, next));

                yield return new TableQuery<TEntity>().Where(filter);
            }
        }

        /// <summary>
        /// Create row key from date time that will be stored in reverse order.
        /// </summary>
        /// <param name="dt">Date time.</param>
        /// <returns>Row key.</returns>
        protected string GetRowKey(DateTime dt)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:D20}", DateTime.MaxValue.Ticks - dt.Ticks);
        }
    }
}