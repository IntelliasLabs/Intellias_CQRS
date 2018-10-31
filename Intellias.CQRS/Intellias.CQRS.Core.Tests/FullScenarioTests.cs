using System.Collections.Generic;
using System.Linq;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Tests.CommandHandlers;
using Intellias.CQRS.Core.Tests.EventHandlers;
using Intellias.CQRS.Core.Tests.QueryHandlers;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Events;
using Intellias.CQRS.Tests.Core.Fakes;
using Intellias.CQRS.Tests.Core.Queries;
using Xunit;

namespace Intellias.CQRS.Core.Tests
{
    /// <summary>
    /// Full CQRS scenario test
    /// </summary>
    public class FullScenarioTests
    {
        /// <summary>
        /// Full demo test
        /// </summary>
        [Fact]
        public void DemoTest()
        {
            var readModelQueryStore = new Dictionary<string, DemoReadModel>();
            var readModelStore = new DemoReadModelStore(readModelQueryStore);
            var demoQueryHandler = new DemoQueryExecutor(readModelStore);

            var createCommand = new TestCreateCommand { TestData = "Test data" };
            var updateCommand = new TestUpdateCommand { TestData = "Test data updated" };
            var deactivateCommand = new TestDeleteCommand();

            var eventHandlers = new DemoEventHandlers(readModelQueryStore);

            var eventBus = new InProcessEventBus();
            eventBus.AddHandler<TestCreatedEvent>(eventHandlers);
            eventBus.AddHandler<TestUpdatedEvent>(eventHandlers);
            eventBus.AddHandler<TestDeletedEvent>(eventHandlers);

            IEventStore eventStore = new InProcessEventStore(eventBus);

            var commandHandlers = new DemoCommandHandlers(eventStore);
            var commandBus = new InProcessCommandBus();
            commandBus.AddHandler<TestCreateCommand>(commandHandlers);
            commandBus.AddHandler<TestUpdateCommand>(commandHandlers);
            commandBus.AddHandler<TestDeleteCommand>(commandHandlers);

            var createResult = commandBus.PublishAsync(createCommand).Result;
            Assert.NotNull(createResult);

            var queryResult = demoQueryHandler.ExecuteQueryAsync(new ReadAllQuery<DemoReadModel>()).Result;
            Assert.Equal(1, queryResult.Count);
            Assert.Equal(createCommand.TestData, queryResult.First().TestData);

            updateCommand.AggregateRootId = queryResult.First().Id;
            updateCommand.ExpectedVersion = 1;
            var updateResult = commandBus.PublishAsync(updateCommand).Result;
            Assert.NotNull(updateResult);

            var updatedQueryResult = demoQueryHandler.ExecuteQueryAsync(new ReadAllQuery<DemoReadModel>()).Result;
            Assert.Equal(updateCommand.TestData, updatedQueryResult.First().TestData);

            deactivateCommand.AggregateRootId = updatedQueryResult.First().Id;
            updateCommand.ExpectedVersion = 1;
            var deactivateResult = commandBus.PublishAsync(deactivateCommand).Result;
            Assert.NotNull(deactivateResult);

            var queryRemovedResult = demoQueryHandler.ExecuteQueryAsync(new ReadAllQuery<DemoReadModel>()).Result;
            Assert.Equal(0, queryRemovedResult.Count);
        }
    }
}
