using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Intellias.CQRS.Pipelines.Transactions;
using Microsoft.Azure.Cosmos.Table;

namespace Intellias.CQRS.Persistence.AzureStorage.Transactions
{
    /// <summary>
    /// Transaction store based on Azure Storage Account Tables.
    /// </summary>
    public class TransactionTableStore : BaseTableStorage2, ITransactionStore
    {
        private const string FlagEntityRowKey = "FlagEntity";
        private const string IsCommittedColumnName = "_IsCommitted";

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionTableStore"/> class.
        /// </summary>
        /// <param name="options">Table storage options.</param>
        public TransactionTableStore(ITableStorageOptions options)
            : base(options, "TransactionStore")
        {
        }

        /// <inheritdoc />
        public Task PrepareAsync(
            string transactionId,
            IReadOnlyCollection<IAggregateRoot> aggregateRoots,
            IIntegrationEvent integrationEvent)
        {
            var events = aggregateRoots.SelectMany(ar => ar.Events).ToList();
            events.Add(integrationEvent);

            var entities = events.Select(e => Serialize(transactionId, e)).ToList();
            entities.Add(new DynamicTableEntity(transactionId, FlagEntityRowKey, "*", new Dictionary<string, EntityProperty>
            {
                { IsCommittedColumnName, new EntityProperty(false) }
            }));

            return ExecuteBatchAsync(entities, (e, batch) => batch.Insert(e));
        }

        /// <inheritdoc />
        public Task CommitAsync(string transactionId)
        {
            var entity = new DynamicTableEntity(transactionId, FlagEntityRowKey, "*", new Dictionary<string, EntityProperty>
            {
                { IsCommittedColumnName, new EntityProperty(true) }
            });

            return MergeAsync(entity);
        }

        /// <summary>
        /// Gets all transactions.
        /// </summary>
        /// <returns>Collection of transactions.</returns>
        public async Task<IReadOnlyCollection<TransactionRecord>> GetAllAsync()
        {
            var entities = await QueryAllSegmentedAsync(new TableQuery<DynamicTableEntity>());
            var partitions = entities.GroupBy(r => r.PartitionKey);

            var records = new List<TransactionRecord>();
            foreach (var partition in partitions)
            {
                var record = new TransactionRecord { TransactionId = partition.Key };
                foreach (var entity in partition)
                {
                    if (entity.RowKey == FlagEntityRowKey)
                    {
                        record.IsCommitted = entity.Properties[IsCommittedColumnName].BooleanValue.GetValueOrDefault();
                        continue;
                    }

                    var @event = Deserialize(entity);
                    if (@event is IIntegrationEvent integrationEvent)
                    {
                        record.IntegrationEvent = integrationEvent;
                        continue;
                    }

                    record.StateEvents.Add(@event);
                }

                records.Add(record);
            }

            return records;
        }

        private IEvent Deserialize(DynamicTableEntity entity)
        {
            return (IEvent)AzureTableSerializer.Deserialize(entity);
        }

        private DynamicTableEntity Serialize(string transactionId, IEvent @event)
        {
            return new DynamicTableEntity(transactionId, GetRowKey(@event.Created))
            {
                Properties = AzureTableSerializer.Serialize(@event, persistType: true)
            };
        }
    }
}