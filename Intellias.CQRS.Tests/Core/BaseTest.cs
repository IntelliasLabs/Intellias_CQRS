using System.Diagnostics;
using Intellias.CQRS.Core.Config;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.EventStore.AzureTable;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Domain;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using Newtonsoft.Json;

namespace Intellias.CQRS.Tests.Core
{
    /// <summary>
    /// Keeps all init data.
    /// </summary>
    public class BaseTest
    {
        static BaseTest()
        {
            // Used to start azure storage emulator process on testing agent
            Process.Start(@"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe", "start")?.WaitForExit();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTest"/> class.
        /// </summary>
        public BaseTest()
        {
            JsonConvert.DefaultSettings = CqrsSettings.JsonConfig;

            BusMock = new Mock<IEventBus>();

            Store = new AzureTableEventStore(CloudStorageAccount.DevelopmentStorageAccount);

            var tableClient = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudTableClient();

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