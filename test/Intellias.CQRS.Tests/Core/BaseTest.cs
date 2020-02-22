using Intellias.CQRS.Core.Config;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.EventStore.AzureTable;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Domain;
using Intellias.CQRS.Tests.Core.Infrastructure;
using Microsoft.Azure.Cosmos.Table;
using Moq;
using Newtonsoft.Json;

namespace Intellias.CQRS.Tests.Core
{
    /// <summary>
    /// Keeps all init data.
    /// </summary>
    public class BaseTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTest"/> class.
        /// </summary>
        public BaseTest()
        {
            JsonConvert.DefaultSettings = CqrsSettings.JsonConfig;

            BusMock = new Mock<IEventBus>();

            var cfg = new TestsConfiguration();
            Store = new AzureTableEventStore(cfg.StorageAccount.ConnectionString);

            var account = CloudStorageAccount.Parse(cfg.StorageAccount.ConnectionString);
            var tableClient = account.CreateCloudTableClient();

            AggregateTable = tableClient.GetTableReference("AggregateStore");
            EventTable = tableClient.GetTableReference(nameof(EventStore));
        }

        /// <summary>
        /// EventStore.
        /// </summary>
        protected IEventStore Store { get; }

        /// <summary>
        /// EventBus.
        /// </summary>
        protected Mock<IEventBus> BusMock { get; }

        /// <summary>
        /// aggregateTable.
        /// </summary>
        protected CloudTable AggregateTable { get; }

        /// <summary>
        /// eventTable.
        /// </summary>
        protected CloudTable EventTable { get; }

        /// <summary>
        /// CreateItem.
        /// </summary>
        /// <param name="id">Id.</param>
        /// /// <param name="testData">Test Data.</param>
        protected void CreateItem(string id, string testData)
        {
            var item = new TestRoot(new TestCreateCommand
            {
                AggregateRootId = id,
                TestData = testData
            });
            Store.SaveAsync(item).Wait();
        }

        /// <summary>
        /// UpdateItem.
        /// </summary>
        /// <param name="id">Id of the item.</param>
        /// <param name="testData">Test Data.</param>
        protected void UpdateItem(string id, string testData)
        {
            var item = new TestRoot(id);

            // Generating virtual load of events..
            var history = Store.GetAsync(id, 0).Result;
            item.LoadFromHistory(history);

            item.Update(new TestUpdateCommand
            {
                AggregateRootId = id,
                TestData = testData
            });
            Store.SaveAsync(item).Wait();
        }
    }
}