using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.EventStore.AzureTable;
using Intellias.CQRS.Tests.Core.CommandHandlers;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.EventHandlers;
using Intellias.CQRS.Tests.Core.Fakes;
using Intellias.CQRS.Tests.Core.Queries;
using Microsoft.WindowsAzure.Storage;
using Xunit;

namespace Intellias.CQRS.Tests.PerformenceTests
{
    public class AzureTableEventStorePerformenceTest
    {
        private readonly InProcessCommandBus commandBus;
        private readonly AzureTableEventStore eventStore;

        public AzureTableEventStorePerformenceTest()
        {
            // Let's use real event Store to test related performence issues with it

            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            eventStore = new AzureTableEventStore(storageAccount);

            // Fake part
            var eventBus = new InProcessEventBus();
            commandBus = new InProcessCommandBus();

            var dictionary = new Dictionary<Type, Dictionary<string, object>>();
            var queryStore = new InProcessQueryStore<TestQueryModel>(dictionary);

            // Business logic setting
            var eventHandlers = new DemoEventHandlers(queryStore);
            eventBus.AddAllHandlers(eventHandlers);

            var commandHandlers = new DemoCommandHandlers(eventStore, eventBus);
            commandBus.AddAllHandlers(commandHandlers);
        }

        [Fact]
        public async Task AzureTableStoreWhenManyCommandsProcessedPerformenceTest()
        {
            // Preconditions: let's create an test entity
            var createCmd = new TestCreateCommand
            {
                AggregateRootId = $"Performence_CQRS_Test_{Unified.NewCode()}",
                TestData = "Initial test data"
            };

            await commandBus.PublishAsync(createCmd);

            // Then let's create and push 10 update commands to the command bus in the same moment
            const int numberOfUpdates = 10;
            var values = new string[numberOfUpdates];

            for (var i = 0; i < numberOfUpdates; i++)
            {
                values[i] = "Some updated value";
            }

            var tasks = values.Select(async value =>
                await commandBus.PublishAsync(
                    new TestUpdateCommand
                    {
                        AggregateRootId = createCmd.AggregateRootId,
                        TestData = value
                    })
            );

            // Act
            Func<Task> act = async () => await Task.WhenAll(tasks);

            // Assert
            act.Should().Throw<StorageException>();

            var events = await eventStore.GetAsync(createCmd.AggregateRootId, 0);

            // Only created event + 1 update event should be written into the store
            events.Count().Should().Be(2);
        }
    }
}
