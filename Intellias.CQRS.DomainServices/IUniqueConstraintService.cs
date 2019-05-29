using System.Threading.Tasks;

namespace Intellias.CQRS.DomainServices
{
    /// <summary>
    /// IUniqueConstraintService
    /// </summary>
    public interface IUniqueConstraintService
    {
        /// <summary>
        /// RemoveStringAsync
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task RemoveStringAsync(string indexName, string value);

        /// <summary>
        /// ReserveStringAsync
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task ReserveStringAsync(string indexName, string value);

        /// <summary>
        /// UpdateStringAsync
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        Task UpdateStringAsync(string indexName, string oldValue, string newValue);
    }
}