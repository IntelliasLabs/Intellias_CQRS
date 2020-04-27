using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Intellias.CQRS.Persistence.AzureStorage.IntegrationEvents;
using Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules;
using Intellias.CQRS.Tests.Utils;
using Intellias.CQRS.Tests.Utils.StorageAccount;
using Xunit;

namespace Intellias.CQRS.Tests.Persistence.AzureStorage.IntegrationEvents
{
    public class IntegrationEventTableStoreTests : StorageAccountTestBase
    {
        private readonly IntegrationEventTableStore store;

        public IntegrationEventTableStoreTests(StorageAccountFixture fixture)
        {
            var options = new DefaultTableStorageOptions
            {
                TableNamePrefix = fixture.ExecutionContext.GetUniqueStorageTablePrefix(),
                ConnectionString = fixture.Configuration.StorageAccount.ConnectionString
            };

            store = new IntegrationEventTableStore(options);
        }

        [Fact]
        public async Task SaveUnpublishedAsync_Always_SavesUnpublishedEvent()
        {
            var integrationEvent = Fixtures.Pipelines.FakeCreatedIntegrationEvent();

            await store.SaveUnpublishedAsync(integrationEvent);
            var record = (await store.GetAllAsync()).First(r => r.IntegrationEvent.Id == integrationEvent.Id);

            record.IsPublished.Should().BeFalse();
            record.IntegrationEvent.Should().BeEquivalentTo(integrationEvent, options => options.ForIntegrationEvent());
        }

        [Fact]
        public async Task MarkAsPublishedAsync_Always_MarksAsPublished()
        {
            var integrationEvent = Fixtures.Pipelines.FakeCreatedIntegrationEvent();

            await store.SaveUnpublishedAsync(integrationEvent);
            await store.MarkAsPublishedAsync(integrationEvent);
            var record = (await store.GetAllAsync()).First(r => r.IntegrationEvent.Id == integrationEvent.Id);

            record.IsPublished.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllAsync_HasMultipleIntegrationEvents_ReturnsAll()
        {
            var integrationEvent1 = Fixtures.Pipelines.FakeCreatedIntegrationEvent();
            var integrationEvent2 = Fixtures.Pipelines.FakeCreatedIntegrationEvent();

            await store.SaveUnpublishedAsync(integrationEvent1);
            await store.SaveUnpublishedAsync(integrationEvent2);
            var storedEvents = (await store.GetAllAsync()).Select(r => r.IntegrationEvent).ToArray();

            storedEvents.Should().ContainEquivalentOf(integrationEvent1, options => options.ForIntegrationEvent());
            storedEvents.Should().ContainEquivalentOf(integrationEvent2, options => options.ForIntegrationEvent());
        }
    }
}