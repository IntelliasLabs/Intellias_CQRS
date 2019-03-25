using System;
using System.Linq;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core;
using Intellias.CQRS.Tests.Core.Events;
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
                TestData = "TestData",
                Timestamp = DateTime.UtcNow
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
            var resultList = Store.GetAllAsync().Result;
            var result = resultList.FirstOrDefault(x => x.Id == model.Id);

            Assert.True(result != null, "Item is not present in DB");
            Assert.True(result.TestData == model.TestData, "Item data is corrupted");
            Assert.True(result.Id == model.Id, "Item Id is corrupted");
        }

        /// <summary>
        /// Update Test
        /// </summary>
        [Fact]
        public void UpdateExistingEntity()
        {
            // Arrange
            var updatedData = Unified.NewCode();

            // Act
            Store.UpdateAsync(model.Id, m => {
                m.TestData = updatedData;
            }).Wait();

            // Assert
            var result = Store.GetAsync(model.Id).Result;

            Assert.Equal(updatedData, result.TestData);
        }

        [Fact]
        public void PreventEventDublication()
        {
            // Arrange
            var e = new TestUpdatedEvent
            {
                AggregateRootId = Unified.NewCode(),
                Id = Unified.NewCode()
            };

            // Act
            Store.ReserveEventAsync(e).Wait();

            // Assert
            Assert.Throws<AggregateException>(() => Store.ReserveEventAsync(e).Wait());
        }
    }
}
