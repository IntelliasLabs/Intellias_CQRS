using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Intellias.CQRS.Persistence.AzureStorage.StateEvents;
using Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules;
using Intellias.CQRS.Tests.Utils;
using Intellias.CQRS.Tests.Utils.Pipelines.Fakes;
using Intellias.CQRS.Tests.Utils.StorageAccount;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Intellias.CQRS.Tests.Persistence.AzureStorage.StateEvents
{
    public class StateEventTableStoreTests : StorageAccountTestBase
    {
        private readonly StateEventTableStore store;

        public StateEventTableStoreTests(StorageAccountFixture fixture)
        {
            var options = new DefaultTableStorageOptions
            {
                TableNamePrefix = fixture.ExecutionContext.GetUniqueStorageTablePrefix(),
                ConnectionString = fixture.Configuration.StorageAccount.ConnectionString
            };

            store = new StateEventTableStore(options);
        }

        [Fact]
        public async Task SaveAsync_NoEvents_Throws()
        {
            var id = Unified.NewCode();
            var executionContext = new AggregateExecutionContext(Fixtures.Pipelines.FakeCreateCommand());
            var aggregateRoot = new FakeAggregateRoot(id, executionContext);

            await store.Awaiting(s => s.SaveAsync(aggregateRoot)).Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task SaveAsync_HasEvents_Stores()
        {
            var id = Unified.NewCode();
            var executionContext = new AggregateExecutionContext(Fixtures.Pipelines.FakeCreateCommand());
            var aggregateRoot = new FakeAggregateRoot(id, executionContext);
            aggregateRoot.Create(FixtureUtils.String());
            aggregateRoot.Update(FixtureUtils.String());

            await store.SaveAsync(aggregateRoot);
            var events = await store.GetAsync(aggregateRoot.Id, 0);

            events.Should().BeEquivalentTo(aggregateRoot.Events, options => options.ForMessage());
        }

        [Fact]
        public async Task SaveAsync_EventHasJsonSerializableProperties_HandlesCorrectly()
        {
            var aggregateRootId = Unified.NewCode();
            var serializableStateEvent = new SerializableEvent
            {
                AggregateRootId = aggregateRootId,
                SerializableObject = new JsonSerializableObject(FixtureUtils.String())
            };

            var aggregateRootMock = new Mock<IAggregateRoot>(MockBehavior.Strict);
            aggregateRootMock.Setup(ar => ar.Id).Returns(aggregateRootId);
            aggregateRootMock.Setup(ar => ar.Events).Returns(new[] { serializableStateEvent });

            await store.SaveAsync(aggregateRootMock.Object);
            var storedEvent = (await store.GetAsync(aggregateRootId, 0)).Single();

            storedEvent.Should().BeEquivalentTo(serializableStateEvent, options => options.ForMessage());
        }

        [Fact]
        public async Task GetAsync_NoEvents_Throws()
        {
            await store.Awaiting(s => s.GetAsync(Unified.NewCode(), 0)).Should().ThrowAsync<KeyNotFoundException>();
        }

        private class SerializableEvent : Event
        {
            public JsonSerializableObject SerializableObject { get; set; }
        }

        private class JsonSerializableObject
        {
            [JsonConstructor]
            public JsonSerializableObject(string data)
            {
                Data = data;
            }

            public string Data { get; }
        }
    }
}