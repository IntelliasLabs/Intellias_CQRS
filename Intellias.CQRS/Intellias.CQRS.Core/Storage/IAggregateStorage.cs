using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// Storage
    /// </summary>
    /// <typeparam name="T">Type of contract</typeparam>
    public interface IAggregateStorage<T> : IDisposable
        where T : IAggregateRoot, new()
    {
        /// <summary>
        /// Create document and saving changes
        /// </summary>
        /// <param name="entity">Entity to Save</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CreateAsync(T entity);

        /// <summary>
        /// Getting one document by native api
        /// </summary>
        /// <param name="aggregateId">Document unique identifier</param>
        /// <param name="version"></param>
        /// <returns>Document</returns>
        Task<T> GetAsync(string aggregateId, int version);
    }
}
