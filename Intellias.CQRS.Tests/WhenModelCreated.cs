using System.Linq;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core;
using Intellias.CQRS.Tests.Core.Queries;
using Xunit;

namespace Intellias.CQRS.Tests
{
    /// <summary>
    /// CRUDTest
    /// </summary>
    public class WhenModelCreated : BaseQueryTest<TestQueryModel>
    {
        private readonly TestQueryModel model;

        /// <summary>
        /// Constructor
        /// </summary>
        public WhenModelCreated()
        {
            model = new TestQueryModel
            {
                Id = Unified.NewCode(),
                ParentId = Unified.Dummy,
                TestData = "TestData"
            };

            Store.CreateAsync(model).Wait();
        }

        /// <summary>
        /// ShouldBeAvailableInGetById
        /// </summary>
        [Fact]
        public void ShouldBeAvailableInGetById()
        {
            var result = Store.GetAsync(model.Id).Result;

            Assert.True(result != null, "Item is not present in DB");
            Assert.True(result.TestData == model.TestData, "Item data is corrupted");
            Assert.True(result.Id == model.Id, "Item Id is corrupted");
        }

        /// <summary>
        /// ShouldBeAvailableInGetById
        /// </summary>
        [Fact]
        public void ShouldBeAvailableInGetAll()
        {
            var resultList = Store.GetAllAsync().Result.Items;
            var result = resultList.FirstOrDefault(x =>x.Id == model.Id);

            Assert.True(result != null, "Item is not present in DB");
            Assert.True(result.TestData == model.TestData, "Item data is corrupted");
            Assert.True(result.Id == model.Id, "Item Id is corrupted");
        }
    }
}
