using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Pipelines.Transactions
{
    /// <summary>
    /// Null object for <see cref="ITransactionScope"/>.
    /// </summary>
    public class NullTransactionScope : ITransactionScope
    {
        /// <inheritdoc />
        public Task<TAggregateRoot?> FindAggregateAsync<TAggregateRoot, TAggregateState>(string aggregateId, AggregateExecutionContext context)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : AggregateState, new()
        {
            throw new InvalidOperationException($"No Aggregate could be retrieved from '{typeof(NullTransactionScope)}'.");
        }

        /// <inheritdoc />
        public TAggregateRoot CreateAggregate<TAggregateRoot, TAggregateState>(string id, AggregateExecutionContext context)
            where TAggregateRoot : BaseAggregateRoot<TAggregateState>
            where TAggregateState : AggregateState, new()
        {
            throw new InvalidOperationException($"'{typeof(NullTransactionScope)}' can't be created and attached.");
        }

        /// <inheritdoc />
        public Task CommitAsync(string transactionId, IIntegrationEvent integrationEvent)
        {
            throw new InvalidOperationException($"'{typeof(NullTransactionScope)}' can't be commited.");
        }
    }
}