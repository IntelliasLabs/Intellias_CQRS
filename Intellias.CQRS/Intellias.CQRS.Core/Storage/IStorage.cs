using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// Storage
    /// </summary>
    /// <typeparam name="T">Type of contract</typeparam>
    public interface IStorage<T> : IDisposable
        where T : IMessage, new()
    {
        /// <summary>
        /// Deleting document and saving changes
        /// </summary>
        /// <param name="id">Document unique identifier</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<T> DeleteAsync(string id);

        /// <summary>
        /// Create document and saving changes
        /// </summary>
        /// <param name="entity">Entity to Save</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<T> CreateAsync(T entity);

        /// <summary>
        /// Update document and saving changes
        /// </summary>
        /// <param name="entity">Entity to Save</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Getting one document by native api
        /// </summary>
        /// <param name="id">Document unique identifier</param>
        /// <returns>Document</returns>
        Task<T> OneAsync(string id);

        /// <summary>
        /// Querying context specific data
        /// </summary>
        /// <param name="predicate">Query filtering predicate for indexed properties</param>
        /// <returns>Data query</returns>
        Task<IReadOnlyCollection<T>> QueryAsync(Expression<Func<T, bool>> predicate = null);
    }
}
