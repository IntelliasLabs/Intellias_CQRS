using System;
using System.Threading.Tasks;

namespace Intellias.CQRS.DomainServices
{
    /// <summary>
    /// IUniqueConstraintService
    /// </summary>
    public interface IUniqueConstraintService
    {
        /// <summary>
        /// Remove constraint from store
        /// </summary>
        /// <param name="indexName">Name of index</param>
        /// <param name="value">Current constraint value</param>
        /// <exception cref="InvalidOperationException">Thrown on error removing constraint</exception>
        /// <returns></returns>
        Task RemoveConstraintAsync(string indexName, string value);

        /// <summary>
        /// Reserve constraint in store
        /// </summary>
        /// <param name="indexName">Name of index</param>
        /// <param name="value">Constraint value</param>
        /// <exception cref="InvalidOperationException">Thrown when new constraint already exists</exception>
        /// <returns></returns>
        Task ReserveConstraintAsync(string indexName, string value);

        /// <summary>
        /// Replaces constraint in store
        /// </summary>
        /// <param name="indexName">Name of index</param>
        /// <param name="oldValue">Current constraint value</param>
        /// <param name="newValue">New constraint value</param>
        /// <exception cref="InvalidOperationException">Thrown when new constraint already exists</exception>
        /// <returns></returns>
        Task UpdateConstraintAsync(string indexName, string oldValue, string newValue);
    }
}