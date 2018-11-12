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
        where TQueryModel : class, IQueryModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TQueryModel> GetAsync(string id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<CollectionQueryModel<TQueryModel>> GetAllAsync();

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
