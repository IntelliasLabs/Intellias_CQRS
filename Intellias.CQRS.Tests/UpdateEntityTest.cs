using System.Linq;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.EventStore.AzureTable.Documents;
using Intellias.CQRS.Tests.Core;
using Intellias.CQRS.Tests.Core.Domain;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Intellias.CQRS.Tests
{
    /// <summary>
    /// UpdateEntityTest
    /// </summary>
    public class UpdateEntityTest : BaseTest
    {
        private readonly string testId = Unified.NewCode();
        private readonly string testData = "Test Data";
        private readonly string testDataUpdated = "Test Data Updated";

        /// <summary>
        /// UpdateEntityTest
        /// </summary>
        public UpdateEntityTest()
        {
            CreateItem(testId, testData);
            UpdateItem(testId, testDataUpdated);
        }

        /// <summary>
        /// Publish method called twice
        /// </summary>
        [Fact]
        public void ShouldCallTwiceServiceBusPublishMethod()
        {
            BusMock.Verify(x =>x.PublishAsync(It.IsAny<IEvent>()), Times.Exactly(2));
        }

        /// <summary>
        /// Check is AggregateRoot record present
        /// </summary>
        [Fact]
        public void ShouldCreateAggregateRootRecordWithVersion2()
        {
            var operation = TableOperation.Retrieve<EventStoreAggregate>(typeof(TestRoot).Name, testId);
            var result = (EventStoreAggregate)AggregateTable.ExecuteAsync(operation).Result.Result;

            Assert.True(result.LastArVersion == 2, "Test version for aggregated root is not equal 2");
        }

        /// <summary>
        /// Check if event serialized 
        /// </summary>
        [Fact]
        public void ShouldCreateEventRecordWithVersion2AndTestData()
        {
            var query = new TableQuery<EventStoreEvent>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, testId));
            var result = EventTable.ExecuteQuerySegmentedAsync(query, null).Result.Results;

            Assert.True(result.Count == 2, "Result event recordset contains incorrect amount of events. Expeted count is 2");

            var record = result.OrderBy(x=>x.Version).Last().Data;
            dynamic @event = JsonConvert.DeserializeObject(record);

            Assert.True(@event.Version == 2, "Test version for updated event is not equal 2");
            Assert.True(@event.TestData == testDataUpdated, "Test data for updated event is lost");
        }
    }
}
