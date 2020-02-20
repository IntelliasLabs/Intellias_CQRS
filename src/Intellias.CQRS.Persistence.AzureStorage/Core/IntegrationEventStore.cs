using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

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

        private class DomainStoreEntity : TableEntity
        {
            public const string EntityPartitionKey = "DomainEntity";

            public DomainStoreEntity(IIntegrationEvent integrationEvent)
                : this()
            {
                TypeName = integrationEvent.GetType().AssemblyQualifiedName;
                PartitionKey = EntityPartitionKey;
                RowKey = GetRowKey(integrationEvent.Created);
                IsCompressed = true;

                var json = JsonConvert.SerializeObject(integrationEvent, TableStorageJsonSerializerSettings.GetDefault());
                Data = IsCompressed ? json.Zip() : json;
            }

            protected DomainStoreEntity()
            {
            }

            public bool IsPublished { get; set; }

            public bool IsCompressed { get; set; }

            public string TypeName { get; set; }

            /// <summary>
            /// Serialized data stored in Table Entity.
            /// </summary>
            public string Data { get; set; }
        }
    }
}