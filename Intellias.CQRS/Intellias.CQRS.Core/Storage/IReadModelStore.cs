using System.Threading.Tasks;

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
}
