using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// IQueryModelWriter - used by event handlers to build query model.
    /// </summary>
    /// <typeparam name="TQueryModel">Query Model Type.</typeparam>
    public interface IQueryModelWriter<TQueryModel>
        where TQueryModel : IQueryModel, new()
    {
        /// <summary>
        /// Delete Read Model Item.
        /// </summary>
        /// <param name="id">Id of Query model.</param>
        /// <returns>Simple Task.</returns>
        Task DeleteAsync(string id);

        /// <summary>
        /// Delete All Read Model Items.
        /// </summary>
        /// <returns>Simple Task.</returns>
        Task ClearAsync();

        /// <summary>
        /// Creates one read model.
        /// </summary>
        /// <param name="queryModel">Query model to create.</param>
        /// <returns>Simple Task.</returns>
        Task CreateAsync(TQueryModel queryModel);

        /// <summary>
        /// Update one read model.
        /// </summary>
        /// <param name="id">Id of Query model.</param>
        /// <param name="updateAction">Update action.</param>
        /// <returns>Simple Task.</returns>
        Task UpdateAsync(string id, Action<TQueryModel> updateAction);

        /// <summary>
        /// Reserve Event Dublication.
        /// </summary>
        /// <param name="event">event for processing.</param>
        /// <returns>Simple Task.</returns>
        Task ReserveEventAsync(IEvent @event);
    }
}
