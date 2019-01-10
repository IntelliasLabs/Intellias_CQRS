using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// Read Model Storage
    /// </summary>
    public interface IQueryModelStore
    {
        /// <summary>
        /// Delete Read Model Item
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(string parentId, string id);
        /// <summary>
        /// Delete Read Model Item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(string id);

        /// <summary>
        /// Delete Read Model
        /// </summary>
        /// <returns></returns>
        Task DeleteAllAsync();
    }
}
