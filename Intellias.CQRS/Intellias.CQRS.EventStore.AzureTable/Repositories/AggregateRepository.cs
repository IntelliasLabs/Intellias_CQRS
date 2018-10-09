using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.EventStore.AzureTable.Documents;
using Intellias.CQRS.EventStore.AzureTable.Extensions;
using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.EventStore.AzureTable.Repositories
{
    /// <summary>
    /// AggregateRepository
    /// </summary>
    public class AggregateRepository
    {
        private readonly CloudTable aggregateTable;

        /// <summary>
        /// Initialize repository
        /// </summary>
        /// <param name="client"></param>
        public AggregateRepository(CloudTableClient client)
        {
            aggregateTable = client.GetTableReference("AggregateStore");
            // Create the CloudTable if it does not exist
            aggregateTable.CreateIfNotExistsAsync().Wait();
        }


        /// <summary>
        /// Get EventStoreAggregate
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<EventStoreAggregate> GetEventStoreAggregate(IAggregateRoot entity)
        {
            if (entity.Events.First().Version == 1)
            {
                var aggregate = entity.ToStoreAggregate();
                await aggregateTable.ExecuteAsync(TableOperation.Insert(aggregate)).ConfigureAwait(false);
                return aggregate;
            }

            var operation = TableOperation.Retrieve<EventStoreAggregate>(entity.GetType().Name, entity.Id);
            var result = await aggregateTable.ExecuteAsync(operation).ConfigureAwait(false);
            return (EventStoreAggregate)result.Result;
        }

        /// <summary>
        /// Merge EventStoreAggregate 
        /// </summary>
        /// <param name="aggregate"></param>
        /// <returns></returns>
        public async Task<EventStoreAggregate> MergeEventStoreAggregate(EventStoreAggregate aggregate)
        {
            var operation = TableOperation.Merge(aggregate);
            await aggregateTable.ExecuteAsync(operation).ConfigureAwait(false);
            return aggregate;
        }

        /// <summary>
        /// Updates version of aggregate
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task UpdateAggregateVersion(IAggregateRoot entity)
        {
            var storeAggregate = await GetEventStoreAggregate(entity).ConfigureAwait(false);
            storeAggregate.LastArVersion = entity.Version;
            await MergeEventStoreAggregate(storeAggregate).ConfigureAwait(false);
        }
    }
}
