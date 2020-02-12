using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.DomainServices
{
    /// <summary>
    /// IUniqueConstraintService.
    /// </summary>
    [Obsolete("Cross-aggregate constraints logic should be handled either via query-side or SAGA choreography")]
    public interface IUniqueConstraintService
    {
        /// <summary>
        /// Remove constraint from store.
        /// </summary>
        /// <param name="indexName">Name of index.</param>
        /// <param name="value">Current constraint value.</param>
        /// <exception cref="InvalidOperationException">Thrown on error removing constraint.</exception>
        /// <returns>Execution Result.</returns>
        Task<IExecutionResult> RemoveConstraintAsync(string indexName, string value);

        /// <summary>
        /// Reserve constraint in store.
        /// </summary>
        /// <param name="indexName">Name of index.</param>
        /// <param name="value">Constraint value.</param>
        /// <exception cref="InvalidOperationException">Thrown when new constraint already exists.</exception>
        /// <returns>Execution Result.</returns>
        Task<IExecutionResult> ReserveConstraintAsync(string indexName, string value);

        /// <summary>
        /// Replaces constraint in store.
        /// </summary>
        /// <param name="indexName">Name of index.</param>
        /// <param name="oldValue">Current constraint value.</param>
        /// <param name="newValue">New constraint value.</param>
        /// <exception cref="InvalidOperationException">Thrown when new constraint already exists.</exception>
        /// <returns>Execution Result.</returns>
        Task<IExecutionResult> UpdateConstraintAsync(string indexName, string oldValue, string newValue);
    }
}