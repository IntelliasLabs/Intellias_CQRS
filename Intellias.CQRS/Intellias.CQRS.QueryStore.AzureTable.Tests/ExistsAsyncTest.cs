using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.QueryStore.AzureTable.Tests.Core;
using Intellias.CQRS.Tests.Core.Queries;
using Xunit;

namespace Intellias.CQRS.QueryStore.AzureTable.Tests
{
    /// <summary>
    /// ExistsAsync method test
    /// </summary>
    public class ExistsAsyncTest : BaseQueryTest<TestQueryModel>
    {
        /// <summary>
        /// ExistsShouldReturnsTrueIfCreated
        /// </summary>
        [Fact]
        public void ExistsShouldReturnsTrueIfCreated()
        {
            var id = Unified.NewCode();
            var model = new TestQueryModel
            {
                Id = id,
                ParentId = Unified.Dummy,
                TestData = "TestData"
            };

            Store.CreateAsync(model).Wait();

            var result = Store.ExistsAsync(id).Result;

            Assert.True(result, "Item should exist in DB");
        }

        /// <summary>
        /// ExistShouldReturnsFalseIfNotCreated
        /// </summary>
        [Fact]
        public void ExistsShouldReturnsFalseIfNotCreated()
        {
            var result = Store.ExistsAsync(Unified.NewCode()).Result;

            Assert.False(result, "Item shouldn't exist in DB");
        }

        /// <summary>
        /// ExistShouldReturnsTrueIfCreated
        /// </summary>
        [Fact]
        public void ExistsShouldReturnsFalseIfCreatedAnotherValue()
        {
            var id = Unified.NewCode();
            var model = new TestQueryModel
            {
                Id = id,
                ParentId = Unified.Dummy,
                TestData = "TestData"
            };

            Store.CreateAsync(model).Wait();
            
            var anotherId = Unified.NewCode();
            var result = Store.ExistsAsync(anotherId).Result;

            Assert.False(result, "Item shouldn't exist in DB");
        }
    }
}
