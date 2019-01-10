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
        /// Get query model by Id
        /// </summary>
        /// <param name="parentId">Identifier of parent node</param>
        /// <param name="id">id of TQueryModel</param>
        /// <returns>Query Model</returns>
        Task<TQueryModel> GetByIdAsync(string parentId, string id);

        /// <summary>
        /// Get all models
        /// </summary>
        /// <returns>Query Model</returns>
        Task<CollectionQueryModel<TQueryModel>> GetAllAsync();

        /// <summary>
        /// Get all models of particular parent
        /// </summary>
        /// <returns>Query Model</returns>
        Task<CollectionQueryModel<TQueryModel>> GetAllAsync(string parentId);
    }
}
