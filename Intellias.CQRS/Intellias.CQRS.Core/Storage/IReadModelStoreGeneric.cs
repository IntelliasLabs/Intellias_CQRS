using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
    public interface IReadModelStore<TReadModel> : IReadModelStore
        where TReadModel : class, IReadModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ReadModelEnvelope<TReadModel>> GetAsync(string id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<ReadCollectionEnvelope<TReadModel>> GetAllAsync();
    }
}
