using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Pipelines.Transactions
{
    /// <summary>
    /// Transactionally store changes in subdomain.
    /// </summary>
    public interface ITransactionStore
    {
        /// <summary>
        /// Saves transaction data.
        /// </summary>
        /// <param name="transactionId">Id of the transaction.</param>
        /// <param name="aggregateRoots">Aggregates changed in a transaction.</param>
        /// <param name="integrationEvent">Transaction integration event.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PrepareAsync(string transactionId, IReadOnlyCollection<IAggregateRoot> aggregateRoots, IIntegrationEvent integrationEvent);

        /// <summary>
        /// Commits transaction.
        /// </summary>
        /// <param name="transactionId">Transaction id.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CommitAsync(string transactionId);
    }
}