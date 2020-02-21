using System;
using System.Linq;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.EventStore.AzureTable.Documents;
using Intellias.CQRS.Tests.Core;
using Microsoft.Azure.Cosmos.Table;
using Xunit;

namespace Intellias.CQRS.Tests
{
    public class WhenEntityCreated : BaseTest
    {
        private readonly string testId = Unified.NewCode();
        private readonly string testData = "Test Data";

        public WhenEntityCreated()
        {
            CreateItem(testId, testData);
        }

        [Fact]
        public void ShouldCreateEventRecordWithVersion1AndTestData()
        {
            var query = new TableQuery<EventStoreEvent>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, testId));
            var result = EventTable.ExecuteQuerySegmentedAsync(query, null).Result.Results;

            var record = result.First();
            dynamic @event = record.Data.FromJson(Type.GetType(record.TypeName));

            Assert.True(@event.Version == 1, "Test version for created event is not equal 1");
            Assert.True(@event.TestData == testData, "Test data for created event is lost");
        }
    }
}
