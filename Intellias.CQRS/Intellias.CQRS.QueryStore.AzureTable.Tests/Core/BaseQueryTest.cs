using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace Intellias.CQRS.QueryStore.AzureTable.Tests.Core
{
    /// <summary>
    /// Base class for Azure Table Storage Query tests
    /// </summary>
    public class BaseQueryTest<TQueryModel> 
        where TQueryModel : class, IQueryModel
    {
        /// <summary>
        /// Query Model Store
        /// </summary>
        protected IQueryModelStore<TQueryModel> Store { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseQueryTest()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            var storeConnectionString = configuration.GetConnectionString("TableStorageConnection");
            Store = new AzureTableQueryStore<TQueryModel>(CloudStorageAccount.Parse(storeConnectionString));
        }
    }
}
