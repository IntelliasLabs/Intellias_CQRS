using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.QueryStore.AzureTable;
using Microsoft.WindowsAzure.Storage;

namespace Intellias.CQRS.Tests.Core
{
    /// <summary>
    /// Base class for Azure Table Storage Query tests
    /// </summary>
    public class BaseQueryTest<TQueryModel> where TQueryModel : class, IQueryModel, new()
    {
        /// <summary>
        /// Query Model Store
        /// </summary>
        protected TableQueryModelStorage<TQueryModel> Store { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseQueryTest()
        {
            Store = new TableQueryModelStorage<TQueryModel>(CloudStorageAccount.DevelopmentStorageAccount);
        }
    }
}
