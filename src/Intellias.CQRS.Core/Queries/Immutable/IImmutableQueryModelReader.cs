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
        Task<TQueryModel> FindAsync(string id, int version);

        /// <summary>
        /// Find latest query model.
        /// Returns NULL if query model is not found.
        /// </summary>
        /// <param name="id">Query model id.</param>
        /// <returns>Found query model.</returns>
        Task<TQueryModel> FindLatestAsync(string id);

        /// <summary>
        /// Finds query model with asked version or closest newer version.
        /// Returns NULL if query model is not found.
        /// </summary>
        /// <param name="id">Query model id.</param>
        /// <param name="fromVersion">From which version to search.</param>
        /// <returns>Found query model.</returns>
        Task<TQueryModel> FindEqualOrLessAsync(string id, int fromVersion);

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
        /// Throws <see cref="KeyNotFoundException"/> if query mode is not found.
        /// </summary>
        /// <param name="id">Query model id.</param>
        /// <returns>Found query model.</returns>
        Task<TQueryModel> GetLatestAsync(string id);

        /// <summary>
        /// Gets query model with asked version or closest older version.
        /// Throws <see cref="KeyNotFoundException"/> if query mode is not found.
        /// </summary>
        /// <param name="id">Query model id.</param>
        /// <param name="fromVersion">From which version to search.</param>
        /// <returns>Found query model.</returns>
        Task<TQueryModel> GetEqualOrLessAsync(string id, int fromVersion);
    }
}