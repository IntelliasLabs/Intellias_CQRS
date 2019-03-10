using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Config;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.EventStore.AzureTable.Documents;
using Intellias.CQRS.EventStore.AzureTable.Extensions;
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
        private readonly CloudTable eventTable;
        private readonly IEventBus eventBus;

        /// <summary>
        /// EventStore
        /// </summary>
        /// <param name="account">Azure Table Storage Account</param>
        /// <param name="eventBus">Event bus for publishing events</param>
        public AzureTableEventStore(CloudStorageAccount account, IEventBus eventBus)
        {
            this.eventBus = eventBus;

            var client = account.CreateCloudTableClient();

            eventTable = client.GetTableReference(nameof(EventStore));

            // Create the CloudTable if it does not exist
            eventTable.CreateIfNotExistsAsync().Wait();
        }


        /// <inheritdoc />
        public async Task SaveAsync(IAggregateRoot entity)
        {
            if (!entity.Events.Any()) { return; }

            await Task.WhenAll(entity.Events
                .Select(async e =>
                {
                    var operation = TableOperation.Insert(e.ToStoreEvent());
                    await eventTable.ExecuteAsync(operation);
                    await eventBus.PublishAsync(e);
                }));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IEvent>> GetAsync(string aggregateId, int fromVersion)
        {
            var query = new TableQuery<EventStoreEvent>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal, aggregateId));

            var results = new List<EventStoreEvent>();
            TableContinuationToken continuationToken = null;

            do
            {
                var queryResults =
                    await eventTable.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = queryResults.ContinuationToken;
                results.AddRange(queryResults.Results);

            } while (continuationToken != null);

            return results.Select(item => JsonConvert.DeserializeObject<IEvent>(item.Data, CqrsSettings.JsonConfig()));
        }
    }
}
