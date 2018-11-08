using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// Generic query handler
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IQueryExecutor<in TQuery, TResult>
    {
        /// <summary>
        /// Execute query
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>Read model</returns>
        Task<TResult> ExecuteQueryAsync(TQuery query);
    }
}
