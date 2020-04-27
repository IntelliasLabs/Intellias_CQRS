using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Microsoft.Azure.Cosmos.Table;

namespace Intellias.CQRS.Persistence.AzureStorage.IntegrationEvents
{
    /// <summary>
    /// Integration event store based on Azure Storage Account Tables.
    /// </summary>
    public class IntegrationEventTableStore : BaseTableStorage2, IIntegrationEventStore
    {
        private const string IsPublishedColumnName = "_IsPublished";

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationEventTableStore"/> class.
        /// </summary>
        /// <param name="options">Table storage options.</param>
        public IntegrationEventTableStore(ITableStorageOptions options)
            : base(options, "IntegrationEventStore")
        {
        }

        /// <inheritdoc />
        public Task SaveUnpublishedAsync(IIntegrationEvent @event)
        {
            var entity = Serialize(@event, false);
            return InsertAsync(entity);
        }

        /// <inheritdoc />
        public Task MarkAsPublishedAsync(IIntegrationEvent @event)
        {
            var entity = new DynamicTableEntity(@event.AggregateRootId, GetRowKey(@event.Created), "*", new Dictionary<string, EntityProperty>
            {
                { IsPublishedColumnName, new EntityProperty(true) }
            });

            return MergeAsync(entity);
        }

        /// <summary>
        /// Gets all integration events.
        /// </summary>
        /// <returns>Collection of integration events.</returns>
        public async Task<IReadOnlyCollection<IntegrationEventRecord>> GetAllAsync()
        {
            var entities = await QueryAllSegmentedAsync(new TableQuery<DynamicTableEntity>());
            return entities.Select(Deserialize).ToArray();
        }

        private DynamicTableEntity Serialize(IIntegrationEvent integrationEvent, bool isPublished)
        {
            return new DynamicTableEntity(integrationEvent.AggregateRootId, GetRowKey(integrationEvent.Created))
            {
                Properties = AzureTableSerializer.Serialize(integrationEvent, persistType: true),
                [IsPublishedColumnName] = new EntityProperty(isPublished)
            };
        }

        private IntegrationEventRecord Deserialize(DynamicTableEntity entity)
        {
            var integrationEvent = (IIntegrationEvent)AzureTableSerializer.Deserialize(entity);
            return new IntegrationEventRecord
            {
                IntegrationEvent = integrationEvent,
                IsPublished = entity.Properties[IsPublishedColumnName].BooleanValue!.Value
            };
        }
    }
}