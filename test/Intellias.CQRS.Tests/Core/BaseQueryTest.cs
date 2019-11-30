using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.QueryStore.AzureTable;
using Intellias.CQRS.Tests.Core.Infrastructure;
using Microsoft.WindowsAzure.Storage;

namespace Intellias.CQRS.Tests.Core
{
    public class BaseQueryTest<TQueryModel>
        where TQueryModel : class, IQueryModel, new()
    {
        public BaseQueryTest()
        {
            var cfg = new TestsConfiguration();
            Store = new TableQueryModelStorage<TQueryModel>(CloudStorageAccount.Parse(cfg.StorageAccount.ConnectionString));
        }

        protected TableQueryModelStorage<TQueryModel> Store { get; }
    }
}
