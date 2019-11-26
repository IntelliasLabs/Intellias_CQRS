using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using Polly;
using Polly.Retry;

namespace Intellias.CQRS.Persistence.AzureStorage.Common
{
    /// <summary>
    /// Proxy for <see cref="CloudTable"/> that handles errors and ensures that table exists.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CloudTableProxy
    {
        private const string ContinuationTokenKey = nameof(ContinuationTokenKey);
        private const string TableKey = nameof(TableKey);
        private const string OperationKey = nameof(OperationKey);
        private const string QueryKey = nameof(QueryKey);

        private static readonly Random Random = new Random(Environment.TickCount);

        // Policy for recreating table for queries and table operations.
        private static readonly AsyncRetryPolicy CreateTablePolicy = Policy
            .Handle<StorageException>(e => e.RequestInformation.ExtendedErrorInformation.ErrorCode == TableErrorCodeStrings.TableNotFound)
            .WaitAndRetryAsync(3, GetJitter, (result, i, context) => CreateTableAsync(context));

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
        /// Executes <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Query to be executed.</param>
        /// <param name="continuationToken">Query continuation token.</param>
        /// <typeparam name="T">Table entity type.</typeparam>
        /// <returns>Query result.</returns>
        public async Task<TableQuerySegment<T>> ExecuteQuerySegmentedAsync<T>(TableQuery<T> query, TableContinuationToken continuationToken)
            where T : ITableEntity, new()
        {
            return await CreateTablePolicy.ExecuteAsync(
                context =>
                {
                    var policyTable = (CloudTable)context[TableKey];
                    var policyQuery = (TableQuery<T>)context[QueryKey];
                    var policyContinuationToken = (TableContinuationToken)context[ContinuationTokenKey];

                    return policyTable.ExecuteQuerySegmentedAsync(policyQuery, policyContinuationToken);
                },
                new Dictionary<string, object>
                {
                    [TableKey] = table,
                    [QueryKey] = query,
                    [ContinuationTokenKey] = continuationToken
                });
        }

        /// <summary>
        /// Executes <paramref name="operation"/>.
        /// </summary>
        /// <param name="operation">Operation to be executed.</param>
        /// <returns>Operation result.</returns>
        public async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            return await CreateTablePolicy.ExecuteAsync(
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

        private static TimeSpan GetJitter(int retryCount)
        {
            return TimeSpan.FromMilliseconds(Math.Pow(2, retryCount) * 50)
                + TimeSpan.FromMilliseconds(Random.Next(0, 200));
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
    }
}