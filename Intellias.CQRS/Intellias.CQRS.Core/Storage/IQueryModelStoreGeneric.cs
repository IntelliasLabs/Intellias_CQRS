using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    public interface IQueryModelStore<TQueryModel> : IQueryModelStore
        where TQueryModel: IQueryModel
    {
        /// <summary>
        /// Returns if the entity exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string id);

        /// <summary>
        /// Returns the root node with particular ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TQueryModel> GetAsync(string id);

        /// <summary>
        /// Returns the child node with particular ID
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TQueryModel> GetAsync(string parentId, string id);

        /// <summary>
        /// Returns all root items in catalog
        /// </summary>
        /// <returns></returns>
        Task<CollectionQueryModel<TQueryModel>> GetAllAsync();

        /// <summary>
        /// Returns children of some parent node
        /// </summary>
        /// <returns></returns>
        Task<CollectionQueryModel<TQueryModel>> GetAllAsync(string parentId);

        /// <summary>
        /// Creates one read model
        /// </summary>
        /// <param name="newQueryModel"></param>
        /// <returns></returns>
        Task<TQueryModel> CreateAsync(TQueryModel newQueryModel);

        /// <summary>
        /// Creates collection of read models
        /// </summary>
        /// <returns></returns>
        Task<CollectionQueryModel<TQueryModel>> CreateAllAsync(IEnumerable<TQueryModel> newCollection);

        /// <summary>
        /// Update one read model
        /// </summary>
        /// <param name="newQueryModel"></param>
        /// <returns></returns>
        Task<TQueryModel> UpdateAsync(TQueryModel newQueryModel);

        /// <summary>
        /// Update collection of read models
        /// </summary>
        /// <returns></returns>
        Task<CollectionQueryModel<TQueryModel>> UpdateAllAsync(IEnumerable<TQueryModel> newCollection);
    }
}
