using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core;
using Intellias.CQRS.Tests.Core.Queries;
using Xunit;

namespace Intellias.CQRS.Tests
{
    /// <summary>
    /// CreateOrUpdateAsync method test
    /// </summary>
    public class CreateOrUpdateAsyncTest : BaseQueryTest<TestQueryModel>
    {
        /// <summary>
        /// CreateOrUpdatesShouldCreatesIfExecutedOnce
        /// </summary>
        [Fact]
        public void CreateOrUpdatesShouldCreatesIfExecutedOnce()
        {
            var id = Unified.NewCode();
            var model = new TestQueryModel
            {
                Id = id,
                ParentId = Unified.Dummy,
                TestData = "TestData"
            };

            Store.CreateOrUpdateAsync(model).Wait();

            var result = Store.GetAsync(id).Result;

            Assert.NotNull(result);
        }

        /// <summary>
        /// CreateOrUpdatesShouldUpdatesIfExecutedTwice
        /// </summary>
        [Fact]
        public void CreateOrUpdatesShouldUpdatesIfExecutedTwice()
        {
            var id = Unified.NewCode();
            var model = new TestQueryModel
            {
                Id = id,
                ParentId = Unified.Dummy,
                TestData = "TestData"
            };

            Store.CreateOrUpdateAsync(model).Wait();

            model.TestData = "NewTestData";

            Store.CreateOrUpdateAsync(model).Wait();

            var result = Store.GetAsync(id).Result;

            Assert.True(result.TestData == model.TestData, "Item should be updated with new testData in DB");
        }
    }
}
