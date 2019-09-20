using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Events;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.Persistence.AzureStorage.Core
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class IntegrationEventStore : IIntegrationEventStore
    {
        private readonly CloudTable table;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationEventStore"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string to events store.</param>
        public IntegrationEventStore(string connectionString)
        {
            var client = CloudStorageAccount
                .Parse(connectionString)
                .CreateCloudTableClient();

            table = client.GetTableReference(nameof(IntegrationEventStore));

            if (!table.ExistsAsync().GetAwaiter().GetResult())
            {
                table.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            }
        }

        /// <inheritdoc />
        public Task SaveUnpublishedAsync(IIntegrationEvent @event)
        {
            return table.ExecuteAsync(TableOperation.Insert(new DomainStoreEntity(@event)));
        }

        /// <inheritdoc />
        public Task MarkAsPublishedAsync(IIntegrationEvent @event)
        {
            var entity = new DynamicTableEntity(DomainStoreEntity.EntityPartitionKey, GetRowKey(@event.Created), "*", new Dictionary<string, EntityProperty>
            {
                { nameof(DomainStoreEntity.IsPublished), new EntityProperty(true) }
            });

            return table.ExecuteAsync(TableOperation.Merge(entity));
        }

        private static string GetRowKey(DateTime created)
        {
            // Build from Created row key that stores data in reverse order.
            return string.Format(CultureInfo.InvariantCulture, "{0:D20}", DateTime.MaxValue.Ticks - created.Ticks);
        }

        private class DomainStoreEntity : TableEntity
        {
            public const string EntityPartitionKey = "DomainEntity";

            public DomainStoreEntity(IIntegrationEvent integrationEvent)
            {
                PartitionKey = EntityPartitionKey;
                RowKey = GetRowKey(integrationEvent.Created);
                TypeName = integrationEvent.GetType().FullName;
                Data = integrationEvent.ToJson();
            }

            public string TypeName { get; set; }

            public string Data { get; set; }

            public bool IsPublished { get; set; }
        }
    }
}