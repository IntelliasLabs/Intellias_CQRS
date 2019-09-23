using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Pipelines.Transactions
{
    /// <summary>
    /// Subdomain transaction scope.
    /// </summary>
    public class TransactionScope : ITransactionScope
    {
        private readonly Dictionary<string, IAggregateRoot> aggregates = new Dictionary<string, IAggregateRoot>();

        private readonly IAggregateStore aggregateStore;
        private readonly IIntegrationEventStore integrationEventStore;
        private readonly ITransactionStore transactionStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScope"/> class.
        /// </summary>
        /// <param name="transactionStore">Transaction store.</param>
        /// <param name="aggregateStore">Aggregate store.</param>
        /// <param name="integrationEventStore">Integration events store.</param>
        public TransactionScope(
            ITransactionStore transactionStore,
            IAggregateStore aggregateStore,
            IIntegrationEventStore integrationEventStore)
        {
            this.aggregateStore = aggregateStore;
            this.integrationEventStore = integrationEventStore;
            this.transactionStore = transactionStore;
        }

        /// <inheritdoc />
        public async Task<TAggregateRoot?> FindAggregateAsync<TAggregateRoot, TAggregateState>(string aggregateId, AggregateExecutionContext context)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : AggregateState, new()
        {
            try
            {
                if (aggregates.TryGetValue(aggregateId, out var abstractAggregate))
                {
                    return (TAggregateRoot)abstractAggregate;
                }

                var aggregate = await aggregateStore.GetAsync<TAggregateRoot, TAggregateState>(aggregateId, context);
                aggregates[aggregateId] = aggregate;
                return aggregate;
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        /// <inheritdoc />
        public TAggregateRoot CreateAggregate<TAggregateRoot, TAggregateState>(string id, AggregateExecutionContext context)
            where TAggregateRoot : BaseAggregateRoot<TAggregateState>
            where TAggregateState : AggregateState, new()
        {
            var aggregate = aggregateStore.Create<TAggregateRoot, TAggregateState>(id, context);

            aggregates[aggregate.Id] = aggregate;

            return aggregate;
        }

        /// <inheritdoc />
        public async Task CommitAsync(string transactionId, IIntegrationEvent integrationEvent)
        {
            // Save side effects to transaction store.
            await transactionStore.PrepareAsync(transactionId, aggregates.Values, integrationEvent);

            // Save changes in aggregates.
            foreach (var aggregate in aggregates.Values)
            {
                await aggregateStore.UpdatedAsync(aggregate);
            }

            // Save domain event as not published.
            await integrationEventStore.SaveUnpublishedAsync(integrationEvent);

            // Mark transaction as commited.
            await transactionStore.CommitAsync(transactionId);

            // Clean tracked aggregates.
            aggregates.Clear();
        }
    }
}