using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Intellias.CQRS.Pipelines.Transactions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Intellias.CQRS.Persistence.AzureStorage.Pipelines
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class TransactionStore : ITransactionStore
    {
        /// <summary>
        /// Table proxy.
        /// </summary>
        private readonly CloudTableProxy tableProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionStore"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string to storage account.</param>
        public TransactionStore(string connectionString)
        {
            var client = CloudStorageAccount
               .Parse(connectionString)
               .CreateCloudTableClient();

            var tableReference = client.GetTableReference(nameof(TransactionStore));

            tableProxy = new CloudTableProxy(tableReference, ensureTableExists: true);
        }

        /// <inheritdoc />
        public Task PrepareAsync(string transactionId, IReadOnlyCollection<IAggregateRoot> aggregateRoots, IIntegrationEvent integrationEvent)
        {
            var events = aggregateRoots.SelectMany(a => a.Events).ToList();
            events.Add(integrationEvent);

            var batchOperation = new TableBatchOperation();
            batchOperation.Insert(new TransactionStoreFlagEntity(transactionId));
            foreach (var @event in events)
            {
                batchOperation.Insert(new TransactionStoreDataEntity(transactionId, @event));
            }

            return tableProxy.ExecuteBatchAsync(batchOperation);
        }

        /// <inheritdoc />
        public Task CommitAsync(string transactionId)
        {
            var entity = new DynamicTableEntity(transactionId, TransactionStoreFlagEntity.EntityRowKey, "*", new Dictionary<string, EntityProperty>
            {
                { nameof(TransactionStoreFlagEntity.IsCommited), new EntityProperty(true) }
            });

            return tableProxy.ExecuteAsync(TableOperation.Merge(entity));
        }

        private class TransactionStoreDataEntity : TableEntity
        {
            public TransactionStoreDataEntity(string transactionId, IEvent @event)
            {
                PartitionKey = transactionId;
                RowKey = GetRowKey(@event.Created);

                TypeName = @event.GetType().AssemblyQualifiedName;
                IsCompressed = true;
                IsPublished = false;

                var json = JsonConvert.SerializeObject(@event, TableStorageJsonSerializerSettings.GetDefault());
                Data = IsCompressed ? json.Zip() : json;
            }

            protected TransactionStoreDataEntity()
            {
            }

            public bool IsPublished { get; set; }

            public bool IsCompressed { get; set; }

            public string TypeName { get; set; }

            /// <summary>
            /// Serialized data stored in Table Entity.
            /// </summary>
            public string Data { get; set; }

            private static string GetRowKey(DateTime created)
            {
                // Build from Created row key that stores data in reverse order.
                return string.Format(CultureInfo.InvariantCulture, "{0:D20}", DateTime.MaxValue.Ticks - created.Ticks);
            }
        }

        private class TransactionStoreFlagEntity : TableEntity
        {
            public const string EntityRowKey = "FlagEntity";

            public TransactionStoreFlagEntity(string transactionId)
            {
                PartitionKey = transactionId;
                RowKey = EntityRowKey;
            }

            public bool IsCommited { get; set; }
        }
    }
}