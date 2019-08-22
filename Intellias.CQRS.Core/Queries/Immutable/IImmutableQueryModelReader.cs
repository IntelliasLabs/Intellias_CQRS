using System.Collections.Generic;
using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Queries.Immutable
{
    /// <summary>
    /// Reads immutable query models of type <typeparamref name="TQueryModel"/>.
    /// </summary>
    /// <typeparam name="TQueryModel">Type of the query model.</typeparam>
    public interface IImmutableQueryModelReader<TQueryModel>
        where TQueryModel : class, IImmutableQueryModel, new()
    {
        /// <summary>
        /// Gets single query model.
        /// Returns NULL if query model is not found.
        /// </summary>
        /// <param name="id">Query model id.</param>
        /// <param name="version">Query model version.</param>
        /// <returns>Found query model or NULL.</returns>
        Task<TQueryModel?> FindAsync(string id, int version);

        /// <summary>
        /// Gets single query model.
        /// Throws <see cref="KeyNotFoundException"/> if query mode is not found.
        /// </summary>
        /// <param name="id">Query model id.</param>
        /// /// <param name="version">Query model version.</param>
        /// <returns>Found query model.</returns>
        Task<TQueryModel> GetAsync(string id, int version);

        /// <summary>
        /// Gets latest query model.
        /// Returns NULL if query model is not found.
        /// </summary>
        /// <param name="id">Query model id.</param>
        /// <returns>Found query model.</returns>
        Task<TQueryModel> GetLatestAsync(string id);
    }
}