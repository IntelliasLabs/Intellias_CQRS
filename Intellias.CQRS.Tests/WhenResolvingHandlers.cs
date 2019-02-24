using System;
using System.Linq;
using Intellias.CQRS.Core.Tools;
using Intellias.CQRS.Tests.Core;
using Intellias.CQRS.Tests.Core.Events;
using Intellias.CQRS.Tests.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Intellias.CQRS.Tests
{
    /// <summary>
    /// WhenResolvingHandlers
    /// </summary>
    public class WhenResolvingHandlers : TestBase
    {
        /// <summary>
        /// ShouldReceiveCompetencyEventHandler
        /// </summary>
        [Fact]
        public void ShouldReceiveCompetencyEventHandler()
        {
            var @event = new TestCreatedEvent();

            var resolver = ServiceProvider.GetService<HandlerDependencyResolver>();

            var result = resolver.ResolveEvent(@event).ToList();

            Assert.True(result.Any(), "Handlers not found in assembly");
            Assert.True(result.First().GetType() == typeof(DemoEventHandlers));
        }


        /// <summary>
        /// ShouldReceiveCompetencyEventHandler
        /// </summary>
        [Fact]
        public void ShouldRaiseErrorOnIllegalAssembly()
        {
            var @event = new TestDeletedEvent();

            var resolver = ServiceProvider.GetService<HandlerDependencyResolver>();

            Assert.Throws<ArgumentNullException>(() =>
                resolver.ResolveEvent(@event).ToList());
        }
    }
}
