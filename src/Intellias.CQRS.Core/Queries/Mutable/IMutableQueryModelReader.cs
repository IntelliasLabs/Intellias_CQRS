using System.Collections.Generic;
using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Queries.Mutable
{
    /// <summary>
    /// Reads mutable query models of type <typeparamref name="TQueryModel"/>.
    /// </summary>
    /// <typeparam name="TQueryModel">Type of the query model.</typeparam>
    public interface IMutableQueryModelReader<TQueryModel>
        where TQueryModel : class, IMutableQueryModel, new()
    {
        /// <summary>
        /// Gets single query model.
        /// Returns NULL if query model is not found.
        /// </summary>
        /// <param name="id">Query model id.</param>
        /// <returns>Found query model or NULL.</returns>
        Task<TQueryModel> FindAsync(string id);

        /// <summary>
        /// Gets single query model.
        /// Throws <see cref="KeyNotFoundException"/> if query mode is not found.
        /// </summary>
        /// <param name="id">Query model id.</param>
        /// <returns>Found query model.</returns>
        Task<TQueryModel> GetAsync(string id);

        /// <summary>
        /// Gets all query models.
        /// </summary>
        /// <returns>Collection of query models.</returns>
        Task<IReadOnlyCollection<TQueryModel>> GetAllAsync();

        /// <summary>
        /// Gets collection of query models by specified ids.
        /// </summary>
        /// <param name="ids">Query model ids.</param>
        /// <returns>Collection of query models.</returns>
        Task<IReadOnlyCollection<TQueryModel>> GetAllAsync(IReadOnlyCollection<string> ids);
    }
}