using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.QueryStore.AzureTable;
using Microsoft.WindowsAzure.Storage;

namespace Intellias.CQRS.Tests.Core
{
    public class BaseQueryTest<TQueryModel>
        where TQueryModel : class, IQueryModel, new()
    {
        public BaseQueryTest()
        {
            Store = new TableQueryModelStorage<TQueryModel>(CloudStorageAccount.DevelopmentStorageAccount);
        }

        protected TableQueryModelStorage<TQueryModel> Store { get; }
    }
}
