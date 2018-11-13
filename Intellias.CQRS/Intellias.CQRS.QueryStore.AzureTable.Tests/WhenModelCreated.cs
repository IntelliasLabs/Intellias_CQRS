using System.Linq;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.QueryStore.AzureTable.Tests.Core;
using Intellias.CQRS.Tests.Core.Queries;
using Xunit;

namespace Intellias.CQRS.QueryStore.AzureTable.Tests
{
    /// <summary>
    /// CRUDTest
    /// </summary>
    public class WhenModelCreated : BaseQueryTest<DemoQueryModel>
    {
        private readonly DemoQueryModel model;

        /// <summary>
        /// Constructor
        /// </summary>
        public WhenModelCreated()
        {
            model = new DemoQueryModel
            {
                Id = Unified.NewCode(),
                TestData = "TestData"
            };

            Store.CreateAsync(model);
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
            var result = resultList.FirstOrDefault(x => x.Id == model.Id);

            Assert.True(result != null, "Item is not present in DB");
            Assert.True(result.TestData == model.TestData, "Item data is corrupted");
            Assert.True(result.Id == model.Id, "Item Id is corrupted");
        }
    }
}
