using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
    /// <typeparam name="TCollectionReadModel"></typeparam>
    public interface IReadModelStore<TReadModel, TCollectionReadModel> : IReadModelStore
        where TReadModel : class, IReadModel
        where TCollectionReadModel: class, IReadModel
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
        Task<TCollectionReadModel> GetAllAsync();
    }
}
