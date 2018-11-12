using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
    public interface IReadModelStore<TReadModel> : IReadModelStore
        where TReadModel : class, IQueryModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TReadModel> GetAsync(string id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<CollectionReadModel<TReadModel>> GetAllAsync();

        /// <summary>
        /// Update one read model
        /// </summary>
        /// <param name="newQueryModel"></param>
        /// <returns></returns>
        Task<TReadModel> UpdateAsync(TReadModel newQueryModel);

        /// <summary>
        /// Update collection of read models
        /// </summary>
        /// <returns></returns>
        Task<CollectionReadModel<TReadModel>> UpdateAllAsync(CollectionReadModel<TReadModel> newCollection);
    }
}
