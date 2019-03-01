using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// IQueryModelReader - used to read query model
    /// </summary>
    public interface IQueryModelReader<TQueryModel> where TQueryModel : IQueryModel
    {
        /// <summary>
        /// Returns the query model item by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TQueryModel> GetAsync(string id);

        /// <summary>
        /// Returns all query model items
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyCollection<TQueryModel>> GetAllAsync();
    }
}
