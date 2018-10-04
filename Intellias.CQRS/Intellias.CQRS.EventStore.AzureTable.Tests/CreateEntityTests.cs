using System.Linq;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.EventStore.AzureTable.Documents;
using Intellias.CQRS.EventStore.AzureTable.Tests.Core;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Intellias.CQRS.EventStore.AzureTable.Tests
{
    /// <summary>
    /// CreateEntityTests
    /// </summary>
    public class WhenEntityCreated : BaseTest
    {
        private readonly string testId = Unified.NewCode();

        /// <summary>
        /// WhenEntityCreated
        /// </summary>
        public WhenEntityCreated()
        {
            CreateItem(testId);
        }

        /// <summary>
        /// Publish method called once
        /// </summary>
        [Fact]
        public void ShouldCallOnceServiceBusPublishMethod()
        {
            BusMock.Verify(x=>x.PublishAsync(It.IsAny<IEvent>()), Times.Once);
        }

        /// <summary>
        /// Check is AggregateRoot record present
        /// </summary>
        [Fact]
        public void ShouldCreateAggregateRootRecordWithVersion1()
        {
            var operation = TableOperation.Retrieve<EventStoreAggregate>(typeof(TestEntity).Name, testId);
            var result = (EventStoreAggregate)AggregateTable.ExecuteAsync(operation).Result.Result;

            Assert.True(result.RowKey == testId && result.LastArVersion == 1);
        }

        /// <summary>
        /// Check if event serialized 
        /// </summary>
        [Fact]
        public void ShouldCreateEventRecordWithVersion1()
        {
            var query = new TableQuery<EventStoreEvent>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, testId));
            var result = EventTable.ExecuteQuerySegmentedAsync(query, null).Result.Results;

            var record = result.First().Data;
            dynamic @event = JsonConvert.DeserializeObject(record);

            Assert.True(@event.Version ==1);
        }
    }
}
