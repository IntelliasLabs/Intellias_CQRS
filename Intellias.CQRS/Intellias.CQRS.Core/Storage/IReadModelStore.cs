using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// 
    /// </summary>
    public interface IReadModelStore
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(string id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task DeleteAllAsync();
    }

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
        Task<ReadCollectionEnvelope<IReadOnlyCollection<TReadModel>>> GetAllAsync();

        //Task UpdateAsync(IReadOnlyCollection<ReadModelUpdate> readModelUpdates,
        //    IReadModelContextFactory readModelContextFactory,
        //    Func<IReadModelContext, IReadOnlyCollection<IEvent>, ReadModelEnvelope<TReadModel>,
        //        Task<ReadModelUpdateResult<TReadModel>>> updateReadModel);
    }
}
