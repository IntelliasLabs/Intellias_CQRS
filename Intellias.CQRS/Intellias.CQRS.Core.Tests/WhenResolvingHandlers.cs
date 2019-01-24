using System.Linq;
using Intellias.CQRS.Core.Tests.Core;
using Intellias.CQRS.Core.Tests.EventHandlers;
using Intellias.CQRS.Core.Tools;
using Intellias.CQRS.Tests.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Intellias.CQRS.Core.Tests
{
    /// <summary>
    /// WhenResolvingHandlers
    /// </summary>
    public class WhenResolvingHandlers : TestBase
    {
        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void ShouldReceiveCompetencyEventHandler()
        {
            var @event = new TestCreatedEvent();

            var resolver = ServiceProvider.GetService<HandlersDependancyResolver>();

            var handlersAssembly = typeof(DemoEventHandlers).Assembly;
            var result = resolver.Resolve(@event, handlersAssembly).ToList();

            Assert.True(result.Any(), "Handlers not found in assembly");
            Assert.True(result.First().GetType() == typeof(DemoEventHandlers));
        }
    }
}
