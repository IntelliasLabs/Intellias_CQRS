using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;
using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.QueryStore.AzureTable
{
    /// <summary>
    /// ITableQueryReader - used to read query model with azure specific features.
    /// </summary>
    /// <typeparam name="TQueryModel">Query Model Type.</typeparam>
    public interface ITableQueryReader<TQueryModel>
        where TQueryModel : IQueryModel, new()
    {
        /// <summary>
        /// Returns all query model items by azure table query.
        /// </summary>
        /// <param name="query">Table Query.</param>
        /// <returns>Collection of Query Models.</returns>
        Task<IReadOnlyCollection<TQueryModel>> GetAllAsync(TableQuery<DynamicTableEntity> query);
    }
}
