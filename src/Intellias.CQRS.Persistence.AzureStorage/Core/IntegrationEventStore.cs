using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.Persistence.AzureStorage.Core
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class IntegrationEventStore : IIntegrationEventStore
    {
        private readonly CloudTableProxy tableProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationEventStore"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string to events store.</param>
        public IntegrationEventStore(string connectionString)
        {
            var client = CloudStorageAccount
                .Parse(connectionString)
                .CreateCloudTableClient();

            var tableReference = client.GetTableReference(nameof(IntegrationEventStore));

            tableProxy = new CloudTableProxy(tableReference, ensureTableExists: true);
        }

        /// <inheritdoc />
        public Task SaveUnpublishedAsync(IIntegrationEvent @event)
        {
            return tableProxy.ExecuteAsync(TableOperation.Insert(new DomainStoreEntity(@event)));
        }

        /// <inheritdoc />
        public Task MarkAsPublishedAsync(IIntegrationEvent @event)
        {
            var entity = new DynamicTableEntity(DomainStoreEntity.EntityPartitionKey, GetRowKey(@event.Created), "*", new Dictionary<string, EntityProperty>
            {
                { nameof(DomainStoreEntity.IsPublished), new EntityProperty(true) }
            });

            return tableProxy.ExecuteAsync(TableOperation.Merge(entity));
        }

        private static string GetRowKey(DateTime created)
        {
            // Build from Created row key that stores data in reverse order.
            return string.Format(CultureInfo.InvariantCulture, "{0:D20}", DateTime.MaxValue.Ticks - created.Ticks);
        }

        private class DomainStoreEntity : BaseJsonTableEntity<IIntegrationEvent>
        {
            public const string EntityPartitionKey = "DomainEntity";

            public DomainStoreEntity(IIntegrationEvent integrationEvent)
                : base(integrationEvent, true)
            {
                PartitionKey = EntityPartitionKey;
                RowKey = GetRowKey(integrationEvent.Created);
            }

            public bool IsPublished { get; set; }

            protected override void SetupDeserializedData(IIntegrationEvent data)
            {
            }
        }
    }
}