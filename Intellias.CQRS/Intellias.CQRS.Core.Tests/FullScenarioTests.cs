using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Storage;
using Intellias.CQRS.Core.Tests.CommandHandlers;
using Intellias.CQRS.Core.Tests.Domain;
using Intellias.CQRS.Core.Tests.EventHandlers;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Events;
using Intellias.CQRS.Tests.Core.Fakes;
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
            var demoCommand = new TestCreateCommand { TestData = "Test data" };

            IEventBus eventBus = new InProcessEventBus<TestCreatedEvent>(new DemoEventHandlers());
            IEventStore eventStore = new InProcessEventStore(eventBus);

            IAggregateStorage<DemoRoot> rootStorage = new InProcessAggregateStorage<DemoRoot>();
            ICommandBus commandBus = new InProcessCommandBus<TestCreateCommand>(new DemoCommandHandlers(rootStorage));

            var result = commandBus.PublishAsync(demoCommand);

            Assert.NotNull(result);
        }
    }
}
