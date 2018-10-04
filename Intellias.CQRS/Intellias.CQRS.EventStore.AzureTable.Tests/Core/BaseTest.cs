using Intellias.CQRS.Core.Config;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using Newtonsoft.Json;

namespace Intellias.CQRS.EventStore.AzureTable.Tests.Core
{
    /// <summary>
    /// Keeps all init data
    /// </summary>
    public class BaseTest
    {
        /// <summary>
        /// EventStore
        /// </summary>
        protected IEventStore Store { get; }

        /// <summary>
        /// EventBus
        /// </summary>
        protected Mock<IEventBus> BusMock { get; }

        /// <summary>
        /// aggregateTable
        /// </summary>
        protected CloudTable AggregateTable { get; }

        /// <summary>
        /// eventTable
        /// </summary>
        protected CloudTable EventTable { get; }

        /// <summary>
        /// Base constructor
        /// </summary>
        public BaseTest()
        {
            JsonConvert.DefaultSettings = CqrsSettings.JsonConfig;

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            var storeConnectionString = configuration.GetConnectionString("TableStorageConnection");

            BusMock = new Mock<IEventBus>();

            Store = new AzureTableEventStore(storeConnectionString, BusMock.Object);

            var tableClient = CloudStorageAccount
                .Parse(storeConnectionString)
                .CreateCloudTableClient();

            AggregateTable = tableClient.GetTableReference("AggregateStore");
            EventTable = tableClient.GetTableReference("EventStore");
        }

        /// <summary>
        /// CreateItem
        /// </summary>
        /// <param name="id"></param>
        /// /// <param name="testData"></param>
        protected void CreateItem(string id, string testData)
        {
            var item = new TestEntity(id);
            item.Create(new TestCreateCommand
            {
                AggregateRootId = id,
                TestData = testData,
                ExpectedVersion = item.Version
            });
            Store.SaveAsync(item).Wait();
        }

    }
}