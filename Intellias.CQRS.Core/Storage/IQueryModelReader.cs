using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// IQueryModelReader - used to read query model.
    /// </summary>
    /// <typeparam name="TQueryModel">Query Model Type.</typeparam>
    public interface IQueryModelReader<TQueryModel>
        where TQueryModel : IQueryModel, new()
    {
        /// <summary>
        /// Returns the query model item by Id.
        /// </summary>
        /// <param name="id">Id of Query model to find.</param>
        /// <returns>Query Model.</returns>
        Task<TQueryModel> GetAsync(string id);

        /// <summary>
        /// Returns all query model items.
        /// </summary>
        /// <returns>Collection of Query Models.</returns>
        Task<IReadOnlyCollection<TQueryModel>> GetAllAsync();
    }
}
