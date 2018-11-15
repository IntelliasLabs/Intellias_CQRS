using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// Generic query executor for read operations
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    public interface IReadQueryExecutor<TQueryModel> where TQueryModel : class, IQueryModel
    {
        /// <summary>
        /// Get query model by Id
        /// </summary>
        /// <param name="id">id of TQueryModel</param>
        /// <returns>Query Model</returns>
        Task<TQueryModel> GetByIdAsync(string id);

        /// <summary>
        /// Get all models of concrete type
        /// </summary>
        /// <returns>Query Model</returns>
        Task<CollectionQueryModel<TQueryModel>> GetAllAsync();
    }
}
