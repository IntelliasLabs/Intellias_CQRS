using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.EventStore.AzureTable.Documents;
using Intellias.CQRS.Tests.Core;
using Microsoft.WindowsAzure.Storage.Table;
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
        }

        /// <summary>
        /// Check if event serialized
        /// </summary>
        [Fact]
        public void ShouldCreateEventRecordWithVersion2AndTestData()
        {
            UpdateItem(testId, testDataUpdated);

            var query = new TableQuery<EventStoreEvent>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, testId));
            var result = EventTable.ExecuteQuerySegmentedAsync(query, null).Result.Results;

            Assert.True(result.Count == 2, "Result event recordset contains incorrect amount of events. Expeted count is 2");

            var record = result.OrderBy(x => x.RowKey).Last();
            dynamic @event = record.Data.FromJson(Type.GetType(record.TypeName));

            Assert.True(@event.Version == 2, "Test version for updated event is not equal 2");
            Assert.True(@event.TestData == testDataUpdated, "Test data for updated event is lost");
        }

        /// <summary>
        /// Verifies if Azure table throws exception if such AR can't be found
        /// </summary>
        [Fact]
        public void ShouldThrowAnExceptionWhenARNotFound()
        {
            Action updateItemAction = () => { UpdateItem("Some Fake AR Id", testDataUpdated); };

            updateItemAction.Should().Throw<KeyNotFoundException>();
        }
    }
}
