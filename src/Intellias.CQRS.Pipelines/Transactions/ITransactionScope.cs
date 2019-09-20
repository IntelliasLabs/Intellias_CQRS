using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Pipelines.Transactions
{
    /// <summary>
    /// Scope of transaction in subdomain.
    /// </summary>
    public interface ITransactionScope
    {
        /// <summary>
        /// Returns aggregate if it exists, otherwise returns null.
        /// </summary>
        /// <param name="aggregateId">Aggregate id.</param>
        /// <param name="context">Aggregate execution context.</param>
        /// <typeparam name="TAggregateRoot">Type of the aggregate.</typeparam>
        /// <typeparam name="TAggregateState">Type of the aggregate state.</typeparam>
        /// <returns>Found aggregate or null.</returns>
        Task<TAggregateRoot> FindAggregateAsync<TAggregateRoot, TAggregateState>(string aggregateId, AggregateExecutionContext context)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : AggregateState, new();

        /// <summary>
        /// Crerates new aggregate to scope to track changes of its state.
        /// </summary>
        /// <param name="id">Id of the aggregate.</param>
        /// <param name="context">Aggregate execution context.</param>
        /// <typeparam name="TAggregateRoot">Type of the aggregate.</typeparam>
        /// <typeparam name="TAggregateState">Type of the aggregate state.</typeparam>
        /// <returns>Newly created Aggregate.</returns>
        TAggregateRoot CreateAggregate<TAggregateRoot, TAggregateState>(string id, AggregateExecutionContext context)
            where TAggregateRoot : BaseAggregateRoot<TAggregateState>
            where TAggregateState : AggregateState, new();

        /// <summary>
        /// Saves changes in the subdomain.
        /// </summary>
        /// <param name="transactionId">Id of the transaction.</param>
        /// <param name="integrationEvent">Integration event.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CommitAsync(string transactionId, IIntegrationEvent integrationEvent);
    }
}