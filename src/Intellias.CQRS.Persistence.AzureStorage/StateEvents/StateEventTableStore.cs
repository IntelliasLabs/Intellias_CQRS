using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Microsoft.Azure.Cosmos.Table;

namespace Intellias.CQRS.Persistence.AzureStorage.StateEvents
{
    /// <summary>
    /// State event store based on Azure Storage Account Tables.
    /// </summary>
    public class StateEventTableStore : BaseTableStorage2, IEventStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateEventTableStore"/> class.
        /// </summary>
        /// <param name="options">Table storage options.</param>
        public StateEventTableStore(ITableStorageOptions options)
            : base(options, "StateEventStore")
        {
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<IEvent>> SaveAsync(IAggregateRoot entity)
        {
            if (entity.Events.Count == 0)
            {
                throw new InvalidOperationException($"Aggregate root '{entity.Id}' have no state events to save.");
            }

            var entities = entity.Events.Select(Serialize).ToArray();
            await ExecuteBatchAsync(entities, (e, batch) => batch.Insert(e));

            return entity.Events;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<IEvent>> GetAsync(string aggregateId, int fromVersion)
        {
            var query = new TableQuery<DynamicTableEntity>()
                .Where(TableQuery.GenerateFilterCondition(nameof(TableEntity.PartitionKey), QueryComparisons.Equal, aggregateId));

            var entities = await QueryAllSegmentedAsync(query);
            if (entities.Count == 0)
            {
                throw new KeyNotFoundException($"No state events are found for aggregate root '{aggregateId}'.");
            }

            return entities.Select(Deserialize).ToArray();
        }

        private static IEvent Deserialize(DynamicTableEntity entity)
        {
            var stateEvent = (IEvent)AzureTableSerializer.Deserialize(entity);
            return stateEvent;
        }

        private DynamicTableEntity Serialize(IEvent stateEvent)
        {
            return new DynamicTableEntity(stateEvent.AggregateRootId, stateEvent.Version.ToString("D9", CultureInfo.InvariantCulture))
            {
                Properties = AzureTableSerializer.Serialize(stateEvent, persistType: true)
            };
        }
    }
}