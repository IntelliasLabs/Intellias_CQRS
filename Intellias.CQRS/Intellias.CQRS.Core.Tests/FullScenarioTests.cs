using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Tests.Commands;
using Intellias.CQRS.Core.Tests.Fakes;
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
            var demoCommand = new DemoCreateCommand { Name = "Test data" };

            ICommandBus commandBus = new InProcessCommandBus();

            var result = commandBus.SendAsync(demoCommand);

            Assert.NotNull(result);
        }
    }
}
