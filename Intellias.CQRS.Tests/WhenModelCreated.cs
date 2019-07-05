using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core;
using Intellias.CQRS.Tests.Core.Events;
using Intellias.CQRS.Tests.Core.Queries;
using Microsoft.WindowsAzure.Storage;
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

            Assert.NotNull(result);
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

            Assert.NotNull(result);
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

        /// <summary>
        /// Update existing entity in parallel
        /// </summary>
        [Fact]
        public void UpdateExistingEntityInParallelAsync()
        {
            const int numberOfUpdates = 10;
            var values = new string[numberOfUpdates];

            for (var i = 0 ; i < numberOfUpdates; i++)
            {
                var currentValue = Unified.NewCode();
                values[i] = currentValue;
            }
                
            var tasks = values.Select(async value =>
                await Store.UpdateAsync(model.Id, m =>
                {
                    m.TestList.Add(value);
                })
            );

            // Act
            Func<Task> act = async () => await Task.WhenAll(tasks);

            // Assert
            act.Should().NotThrow<StorageException>();

            var queryModel = Store.GetAsync(model.Id).Result;

            // Verify that all values are presented in list
            // It means that all updates are completed successfully
            foreach(var val in values)
            {
                queryModel.TestList.Should().Contain(val);
            }
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
