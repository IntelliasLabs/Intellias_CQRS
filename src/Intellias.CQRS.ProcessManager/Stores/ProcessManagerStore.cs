using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Intellias.CQRS.ProcessManager.Pipelines.Response;
using Intellias.CQRS.QueryStore.AzureTable.Options;
using Microsoft.Azure.Cosmos.Table;

namespace Intellias.CQRS.ProcessManager.Stores
{
    /// <summary>
    /// Process store.
    /// </summary>
    public class ProcessManagerStore : IProcessManagerStore
    {
        private const string IsPersitedColumnName = "_IsPersisted";

        private readonly CloudTableProxy tableProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessManagerStore"/> class.
        /// </summary>
        /// <param name="options">Table store option.</param>
        public ProcessManagerStore(TableStorageOptions options)
        {
            var client = CloudStorageAccount
               .Parse(options.ConnectionString)
               .CreateCloudTableClient();

            var tableName = options.TableNamePrefix + nameof(ProcessManagerStore);
            var tableReference = client.GetTableReference(tableName);

            tableProxy = new CloudTableProxy(tableReference, ensureTableExists: true);
        }

        /// <inheritdoc/>
        public Task MarkMessageAsPublishedAsync(string id, IMessage message)
        {
            var entity = new DynamicTableEntity(id, message.Id, "*", new Dictionary<string, EntityProperty>
            {
                { IsPersitedColumnName, new EntityProperty(true) }
            });

            return tableProxy.ExecuteAsync(TableOperation.Merge(entity));
        }

        /// <inheritdoc/>
        public Task PersistMessagesAsync(string id, IReadOnlyCollection<IMessage> messages)
        {
            var batchOperation = new TableBatchOperation();
            foreach (var msg in messages)
            {
                var properties = AzureTableSerializer.Serialize(msg, true);
                properties.Add(IsPersitedColumnName, new EntityProperty(false));

                batchOperation.Insert(new DynamicTableEntity(id, msg.Id, "*", properties));
            }

            return tableProxy.ExecuteBatchAsync(batchOperation);
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyCollection<ProcessMessage>> GetMessagesAsync(string id)
        {
            var filterByPartition = TableQuery.GenerateFilterCondition(nameof(TableEntity.PartitionKey), QueryComparisons.Equal, id);

            var operation = new TableQuery<DynamicTableEntity>()
                .Where(filterByPartition);

            var tableResult = await tableProxy.ExecuteQuerySegmentedAsync(operation, null);
            var processMessages = new List<ProcessMessage>();
            foreach (var entity in tableResult.Results)
            {
                var isPublished = entity.Properties.TryGetValue(IsPersitedColumnName, out var value) && value.BooleanValue.GetValueOrDefault();
                var message = (IMessage)AzureTableSerializer.Deserialize(entity);

                processMessages.Add(new ProcessMessage(message, isPublished));
            }

            return processMessages.ToArray();
        }
    }
}