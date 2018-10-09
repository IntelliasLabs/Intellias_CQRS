using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.EventStore.AzureTable.Repositories;
using Microsoft.WindowsAzure.Storage;

namespace Intellias.CQRS.EventStore.AzureTable
{
    /// <inheritdoc />
    /// <summary>
    /// Azure Table Storage event store
    /// </summary>
    public class AzureTableEventStore : IEventStore
    {
        private readonly AggregateRepository aggregateRepository;
        private readonly EventRepository eventRepository;
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

            aggregateRepository = new AggregateRepository(client);
            eventRepository = new EventRepository(client);
        }


        /// <inheritdoc />
        public async Task SaveAsync(IAggregateRoot entity)
        {
            if (!entity.Events.Any()) { return; }

            await Task.WhenAll(entity.Events
                .Select(async e =>
                {
                    await eventRepository.InsertEvent(e).ConfigureAwait(false);
                    await eventBus.PublishAsync(e).ConfigureAwait(false);
                })).ConfigureAwait(false);

            await aggregateRepository.UpdateAggregateVersion(entity).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IEvent>> GetAsync(string aggregateId, int version)
        {
            // ToDo: Implement snapshot logic here!
            return await eventRepository.GetEvents(aggregateId/*, version*/).ConfigureAwait(false);
        }
    }
}
