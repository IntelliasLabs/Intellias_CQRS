using Intellias.CQRS.Core.Events;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Intellias.CQRS.EventStore.AzureTable.Tests.Core
{
    /// <summary>
    /// Keeps all init data
    /// </summary>
    public class BaseTest
    {
        protected IEventStore store;
        protected IEventBus bus;

        /// <summary>
        /// Base constructor
        /// </summary>
        public BaseTest()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();


            bus = Mock.Of<IEventBus>();
            store = new AzureTableEventStore(
                configuration.GetConnectionString("TableStorageConnection"), 
                bus);
        }



    }
}