using System.Collections.Generic;
using System.Diagnostics;
using Intellias.CQRS.Core.Config;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.EventStore.AzureTable;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Domain;
using Intellias.CQRS.Tests.Core.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using Newtonsoft.Json;

namespace Intellias.CQRS.Tests.Core
{
    /// <summary>
    /// Keeps all init data
    /// </summary>
    public class BaseTest
    {
        static BaseTest()
        {
            // Used to start azure storage emulator process on testing agent
            Process.Start(@"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe", "start").WaitForExit();
        }

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
            EventTable = tableClient.GetTableReference(nameof(EventStore));
        }

        /// <summary>
        /// CreateItem
        /// </summary>
        /// <param name="id"></param>
        /// /// <param name="testData"></param>
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
        /// UpdateItem
        /// </summary>
        /// <param name="id"></param>
        /// <param name="testData"></param>
        protected void UpdateItem(string id, string testData)
        {
            var item = new TestRoot(id);

            // Generating virtual load of events..
            item.LoadFromHistory(new List<IEvent>
            {
                new TestCreatedEvent
                {
                    AggregateRootId = id,
                    Version = 1,
                    TestData = "Data to be updated...."
                }
            });

            item.Update(new TestUpdateCommand
            {
                AggregateRootId = id,
                TestData = testData
            });
            Store.SaveAsync(item).Wait();
        }
    }
}