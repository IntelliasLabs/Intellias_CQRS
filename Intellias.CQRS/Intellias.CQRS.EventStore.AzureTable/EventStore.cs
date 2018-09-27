using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.EventStore.AzureTable.Documents;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Intellias.CQRS.EventStore.AzureTable
{
    /// <inheritdoc />
    /// <summary>
    /// Azure Table Storage event store
    /// </summary>
    public class AzureTableEventStore : IEventStore
    {
        private readonly CloudTable _eventTable;

        /// <summary>
        /// EventStore
        /// </summary>
        /// <param name="connectionString">Azure Table Storage connection string</param>
        public AzureTableEventStore(string connectionString)
        {
            var client = CloudStorageAccount
                .Parse(connectionString)
                .CreateCloudTableClient();

            _eventTable = client.GetTableReference("EventStore");

            // Create the CloudTable if it does not exist
            _eventTable.CreateIfNotExistsAsync().Wait();
        }


        /// <inheritdoc />
        public async Task SaveAsync(IAggregateRoot entity)
        {
            var items = entity.Events
                .Select(e => new EventStoreItem
                {
                    PartitionKey = e.AggregateRootId,
                    RowKey = Unified.NewCode(),
                    Data = JsonConvert.SerializeObject(e, Formatting.Indented),
                    EventType = e.GetType().Name,
                    ETag = "*"
                });

            foreach (var item in items)
            {
                var operation = TableOperation.Insert(item);
                await _eventTable.ExecuteAsync(operation);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IEvent>> GetAsync(string aggregateId, int version)
        {
            var query = new TableQuery<EventStoreItem>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal, aggregateId));

            var results = new List<EventStoreItem>();
            TableContinuationToken continuationToken = null;

            do
            {
                var queryResults =
                    await _eventTable.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = queryResults.ContinuationToken;
                results.AddRange(queryResults.Results);

            } while (continuationToken != null);

            return results
                .Select(r=>(IEvent)JsonConvert.DeserializeObject(r.Data, Type.GetType(r.EventType)))
                .ToList();
        }
    }
}
