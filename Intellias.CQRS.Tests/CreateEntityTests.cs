using System.Linq;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.EventStore.AzureTable.Documents;
using Intellias.CQRS.Tests.Core;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Xunit;

namespace Intellias.CQRS.Tests
{
    /// <summary>
    /// CreateEntityTests
    /// </summary>
    public class WhenEntityCreated : BaseTest
    {
        private readonly string testId = Unified.NewCode();
        private readonly string testData = "Test Data";

        /// <summary>
        /// WhenEntityCreated
        /// </summary>
        public WhenEntityCreated()
        {
            CreateItem(testId, testData);
        }

        /// <summary>
        /// Check if event serialized
        /// </summary>
        [Fact]
        public void ShouldCreateEventRecordWithVersion1AndTestData()
        {
            var query = new TableQuery<EventStoreEvent>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, testId));
            var result = EventTable.ExecuteQuerySegmentedAsync(query, null).Result.Results;

            var record = result.First().Data;
            dynamic @event = JsonConvert.DeserializeObject(record);

            Assert.True(@event.Version == 1, "Test version for created event is not equal 1");
            Assert.True(@event.TestData == testData, "Test data for created event is lost");
        }
    }
}
