using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// IQueryModelWriter - used by event handlers to build query model
    /// </summary>
    public interface IQueryModelWriter<in TQueryModel> where TQueryModel : IQueryModel
    {
        /// <summary>
        /// Delete Read Model Item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(string id);

        /// <summary>
        /// Delete All Read Model Items
        /// </summary>
        /// <returns></returns>
        Task ClearAsync();

        /// <summary>
        /// Creates one read model
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        Task CreateAsync(TQueryModel queryModel);

        /// <summary>
        /// Update one read model
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        Task UpdateAsync(TQueryModel queryModel);
    }
}
