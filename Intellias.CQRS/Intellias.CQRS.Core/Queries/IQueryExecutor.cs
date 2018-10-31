using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// Query handler
    /// </summary>
    public interface IQueryExecutor
    {
    }

    /// <summary>
    /// Generic query handler
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IQueryExecutor<in TQuery, TResult> : IQueryExecutor
        where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Execute query
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>Read model</returns>
        Task<TResult> ExecuteQueryAsync(TQuery query);
    }
}
