using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Polly;
using Polly.Retry;

namespace Intellias.CQRS.QueryStore.AzureTable.Common
{
    public class CloudTableProxy
    {
        private const string TableKey = nameof(TableKey);
        private const string OperationKey = nameof(OperationKey);
        private const string QueryKey = nameof(QueryKey);
        private const string ContinuationTokenKey = nameof(ContinuationTokenKey);

        private static readonly Random Random = new Random(Environment.TickCount);

        // Policy for recreating table for operations.
        private static readonly AsyncRetryPolicy<TableResult> ExecuteCreateTablePolicy = Policy
            .HandleResult<TableResult>(r => r.HttpStatusCode == (int)HttpStatusCode.NotFound)
            .Or<StorageException>(e => e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
            .WaitAndRetryAsync(3, Jitter, (result, i, context) => CreateTableAsync(context));

        // Policy for recreating table for segmented queries.
        private static readonly AsyncRetryPolicy ExecuteQuerySegmentedCreateTablePolicy = Policy
            .Handle<StorageException>(e => e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
            .WaitAndRetryAsync(3, Jitter, (result, i, context) => CreateTableAsync(context));

        private readonly CloudTable table;

        public CloudTableProxy(CloudTable table)
        {
            this.table = table;
        }

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

        public async Task<TableQuerySegment<T>> ExecuteQuerySegmentedAsync<T>(TableQuery<T> query, TableContinuationToken? continuationToken)
            where T : ITableEntity, new()
        {
            return await ExecuteQuerySegmentedCreateTablePolicy.ExecuteAsync(
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

        private static async Task CreateTableAsync(Context context)
        {
            if (await ((CloudTable)context[TableKey]).ExistsAsync())
            {
                return;
            }

            await ((CloudTable)context[TableKey]).CreateIfNotExistsAsync();
        }

        private static TimeSpan Jitter(int retryAttempt)
        {
            return TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 50)
                + TimeSpan.FromMilliseconds(Random.Next(0, 200));
        }
    }
}