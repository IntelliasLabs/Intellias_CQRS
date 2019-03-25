using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// IQueryModelWriter - used by event handlers to build query model
    /// </summary>
    public interface IQueryModelWriter<TQueryModel> where TQueryModel : IQueryModel, new()
    {
        /// <summary>
        /// Delete Read Model Item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(string id);

        /// <summary>
        /// Delete All Read Model Items
        /// </summary>
        /// <returns></returns>
        Task ClearAsync();

        /// <summary>
        /// Creates one read model
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        Task CreateAsync(TQueryModel queryModel);

        /// <summary>
        /// Update one read model
        /// </summary>
        /// <param name="updateAction"></param>
        /// <param name="id">ID of query model</param>
        /// <returns></returns>
        Task UpdateAsync(string id, Action<TQueryModel> updateAction);

        /// <summary>
        /// Reserve Event Dublication
        /// </summary>
        /// <param name="event">event for processing</param>
        /// <returns></returns>
        Task ReserveEventAsync(IEvent @event);
    }
}
