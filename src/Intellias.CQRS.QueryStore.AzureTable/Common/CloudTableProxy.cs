using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using Polly;
using Polly.Retry;

namespace Intellias.CQRS.QueryStore.AzureTable.Common
{
    /// <summary>
    /// Proxy for <see cref="CloudTable"/> that handles errors and ensures that table exists.
    /// </summary>
    public class CloudTableProxy
    {
        private const string TableKey = nameof(TableKey);
        private const string OperationKey = nameof(OperationKey);
        private const string QueryKey = nameof(QueryKey);
        private const string ContinuationTokenKey = nameof(ContinuationTokenKey);

        private static readonly Random Random = new Random(Environment.TickCount);

        // Policy for recreating table for queries.
        private static readonly AsyncRetryPolicy ExecuteCreateTablePolicy = Policy
            .Handle<StorageException>(e => e.RequestInformation.ExtendedErrorInformation.ErrorCode == TableErrorCodeStrings.TableNotFound)
            .WaitAndRetryAsync(3, Jitter, (result, i, context) => CreateTableAsync(context));

        private readonly CloudTable table;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudTableProxy"/> class.
        /// </summary>
        /// <param name="table">Reference to Storage Table.</param>
        /// <param name="ensureTableExists">Immediately ensures that table exists.</param>
        public CloudTableProxy(CloudTable table, bool ensureTableExists)
        {
            this.table = table;

            if (ensureTableExists)
            {
                CreateTableAsync(this.table).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Executes <paramref name="operation"/>.
        /// </summary>
        /// <param name="operation">Operation to be executed.</param>
        /// <returns>Operation result.</returns>
        public async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            return await ExecuteCreateTablePolicy.ExecuteAsync(
                context =>
                {
                    var policyTable = (CloudTable)context[TableKey];
                    var policyOperation = (TableOperation)context[OperationKey];

                    return policyTable.ExecuteAsync(policyOperation);
                },
                new Dictionary<string, object>
                {
                    [TableKey] = table,
                    [OperationKey] = operation
                });
        }

        /// <summary>
        /// Executes <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Query to be executed.</param>
        /// <param name="continuationToken">Query continuation token.</param>
        /// <typeparam name="T">Table entity type.</typeparam>
        /// <returns>Query result.</returns>
        public async Task<TableQuerySegment<T>> ExecuteQuerySegmentedAsync<T>(TableQuery<T> query, TableContinuationToken? continuationToken)
            where T : ITableEntity, new()
        {
            return await ExecuteCreateTablePolicy.ExecuteAsync(
                context =>
                {
                    var policyTable = (CloudTable)context[TableKey];
                    var policyQuery = (TableQuery<T>)context[QueryKey];
                    var policyContinuationToken = (TableContinuationToken?)context[ContinuationTokenKey];

                    return policyTable.ExecuteQuerySegmentedAsync(policyQuery, policyContinuationToken);
                },
                new Dictionary<string, object?>
                {
                    [TableKey] = table,
                    [QueryKey] = query,
                    [ContinuationTokenKey] = continuationToken
                });
        }

        private static Task CreateTableAsync(Context context)
        {
            return CreateTableAsync((CloudTable)context[TableKey]);
        }

        private static async Task CreateTableAsync(CloudTable table)
        {
            if (await table.ExistsAsync())
            {
                return;
            }

            await table.CreateIfNotExistsAsync();
        }

        private static TimeSpan Jitter(int retryAttempt)
        {
            return TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 50)
                + TimeSpan.FromMilliseconds(Random.Next(0, 200));
        }
    }
}