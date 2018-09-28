using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Domain.Exceptions;
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
        /// <param name="storeConnectionString">Azure Table Storage connection string</param>
        /// <param name="eventBus">Event bus for publishing events</param>
        public AzureTableEventStore(string storeConnectionString, IEventBus eventBus)
        {
            this.eventBus = eventBus;

            var client = CloudStorageAccount
                .Parse(storeConnectionString)
                .CreateCloudTableClient();

            eventTable = client.GetTableReference("EventStore");

            // Create the CloudTable if it does not exist
            eventTable.CreateIfNotExistsAsync().Wait();
        }


        /// <inheritdoc />
        public async Task SaveAsync(IAggregateRoot entity)
        {
            await Task.WhenAll(entity.Events
                .Select(SaveAndPublishEventAsync));
        }

        private async Task SaveAndPublishEventAsync(IEvent @event)
        {
            var operation = TableOperation.Insert(@event.ToStoreItem());
            await eventTable.ExecuteAsync(operation);
            await eventBus.PublishAsync(@event);
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
                    await eventTable.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = queryResults.ContinuationToken;
                results.AddRange(queryResults.Results);

            } while (continuationToken != null);

            if (!results.Any())
            {
                throw new AggregateNotFoundException("Aggregate Id contains no events, so it is not yet created!");
            }

            return results.Select(item => (IEvent)JsonConvert.DeserializeObject(item.Data, Type.GetType(item.EventType)));
        }
    }
}
