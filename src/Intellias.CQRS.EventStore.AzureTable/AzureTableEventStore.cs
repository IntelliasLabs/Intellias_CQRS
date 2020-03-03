using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.EventStore.AzureTable.Documents;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Microsoft.Azure.Cosmos.Table;

namespace Intellias.CQRS.EventStore.AzureTable
{
    /// <summary>
    /// Azure Table Storage event store.
    /// </summary>
    public class AzureTableEventStore : IEventStore
    {
        private readonly CloudTableProxy tableProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTableEventStore"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string to table..</param>
        public AzureTableEventStore(string connectionString)
        {
            var client = CloudStorageAccount
                .Parse(connectionString)
                .CreateCloudTableClient();

            var tableReference = client.GetTableReference(nameof(EventStore));

            tableProxy = new CloudTableProxy(tableReference, ensureTableExists: true);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<IEvent>> SaveAsync(IAggregateRoot entity)
        {
            if (!entity.Events.Any())
            {
                throw new InvalidOperationException("No events for serialization.");
            }

            var batchOperation = new TableBatchOperation();
            entity.Events.ToList().ForEach(e => batchOperation.Insert(new EventStoreEvent(e)));
            await tableProxy.ExecuteBatchAsync(batchOperation);

            return entity.Events;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<IEvent>> GetAsync(string aggregateId, int fromVersion)
        {
            var query = new TableQuery<EventStoreEvent>()
                .Where(TableQuery.GenerateFilterCondition(
                    "PartitionKey",
                    QueryComparisons.Equal,
                    aggregateId));

            var results = new List<IEvent>();
            var continuationToken = new TableContinuationToken();

            do
            {
                var queryResults = await tableProxy.ExecuteQuerySegmentedAsync(query, continuationToken);

                if (!queryResults.Results.Any())
                {
                    throw new KeyNotFoundException($"Aggregate Root with id = '{aggregateId}' hasn't been found");
                }

                continuationToken = queryResults.ContinuationToken;
                results.AddRange(queryResults.Results.Select(tableEntity => tableEntity.ToEvent()));
            }
            while (continuationToken != null);

            return results.AsReadOnly();
        }
    }
}
