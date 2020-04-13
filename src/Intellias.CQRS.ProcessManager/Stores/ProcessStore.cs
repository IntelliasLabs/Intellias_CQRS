using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Intellias.CQRS.ProcessManager.Pipelines.Response;
using Intellias.CQRS.QueryStore.AzureTable.Options;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;

namespace Intellias.CQRS.ProcessManager.Stores
{
    /// <summary>
    /// Process store.
    /// </summary>
    /// <typeparam name="TProcessHandler">Process handler type.</typeparam>
    public class ProcessStore<TProcessHandler> : IProcessStore<TProcessHandler>
        where TProcessHandler : BaseProcessHandler
    {
        private const string IsPersitedColumnName = "_IsPersisted";
        private const string IsPublishedColumnName = "_IsPublished";

        private readonly CloudTableProxy tableProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessStore{TProcessHandler}"/> class.
        /// </summary>
        /// <param name="options">Table store option.</param>
        public ProcessStore(IOptionsMonitor<TableStorageOptions> options)
        {
            var client = CloudStorageAccount
               .Parse(options.CurrentValue.ConnectionString)
               .CreateCloudTableClient();

            var tableName = options.CurrentValue.TableNamePrefix + typeof(TProcessHandler).Name;
            var tableReference = client.GetTableReference(tableName);

            tableProxy = new CloudTableProxy(tableReference, ensureTableExists: true);
        }

        /// <inheritdoc/>
        public Task MarkMessageAsPublishedAsync(string id, IMessage message)
        {
            var entity = new DynamicTableEntity(id, message.Id, "*", new Dictionary<string, EntityProperty>
            {
                { IsPublishedColumnName, new EntityProperty(true) },
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
                properties.Add(IsPublishedColumnName, new EntityProperty(false));

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
                var isPublished = entity.Properties.TryGetValue(IsPublishedColumnName, out var published) && published.BooleanValue.GetValueOrDefault();
                var isPersisted = entity.Properties.TryGetValue(IsPersitedColumnName, out var persited) && persited.BooleanValue.GetValueOrDefault();

                var message = (IMessage)AzureTableSerializer.Deserialize(entity);

                processMessages.Add(new ProcessMessage(message, isPublished || isPersisted));
            }

            return processMessages.ToArray();
        }
    }
}