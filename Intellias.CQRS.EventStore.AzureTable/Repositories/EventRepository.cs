using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Config;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.EventStore.AzureTable.Documents;
using Intellias.CQRS.EventStore.AzureTable.Extensions;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Intellias.CQRS.EventStore.AzureTable.Repositories
{
    /// <summary>
    /// EventRepository
    /// </summary>
    public class EventRepository
    {
        private readonly CloudTable eventTable;

        /// <summary>
        /// EventRepository init
        /// </summary>
        /// <param name="client"></param>
        public EventRepository(CloudTableClient client)
        {
            eventTable = client.GetTableReference(nameof(EventStore));

            // Create the CloudTable if it does not exist
            eventTable.CreateIfNotExistsAsync().Wait();
        }

        /// <summary>
        /// GetEvents
        /// </summary>
        /// <param name="aggregateId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IEvent>> GetEvents(string aggregateId/*, int version*/)
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

        /// <summary>
        /// InsertEvent
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public Task InsertEvent(IEvent @event)
        {
            var operation = TableOperation.Insert(@event.ToStoreEvent());
            return eventTable.ExecuteAsync(operation);
        }
    }
}
