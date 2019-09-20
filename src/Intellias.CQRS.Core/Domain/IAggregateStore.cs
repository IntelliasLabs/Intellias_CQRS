using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Domain
{
    /// <summary>
    /// Store of aggregates.
    /// </summary>
    public interface IAggregateStore
    {
        /// <summary>
        /// Creates aggregate.
        /// </summary>
        /// <param name="aggregateId">Aggregate id.</param>
        /// <param name="context">Execution context of the aggregate.</param>
        /// <typeparam name="TAggregateRoot">Type of the aggregate.</typeparam>
        /// <typeparam name="TAggregateState">Type of the aggregate state.</typeparam>
        /// <returns>Found aggregate.</returns>
        Task<TAggregateRoot> GetAsync<TAggregateRoot, TAggregateState>(string aggregateId, AggregateExecutionContext context)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : AggregateState, new();

        /// <summary>
        /// Creates aggregate.
        /// </summary>
        /// <param name="aggregateId">Aggregate id.</param>
        /// <param name="context">Execution context of the aggregate.</param>
        /// <typeparam name="TAggregateRoot">Type of the aggregate.</typeparam>
        /// <typeparam name="TAggregateState">Type of the aggregate state.</typeparam>
        /// <returns>Found aggregate.</returns>
        TAggregateRoot Create<TAggregateRoot, TAggregateState>(string aggregateId, AggregateExecutionContext context)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : AggregateState, new();

        /// <summary>
        /// Saves aggregate.
        /// </summary>
        /// <param name="aggregate">Aggregate.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task UpdatedAsync(IAggregateRoot aggregate);
    }
}