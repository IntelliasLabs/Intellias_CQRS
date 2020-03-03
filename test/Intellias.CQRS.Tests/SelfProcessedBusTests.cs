using System;
using System.Collections.Generic;
using System.Reflection;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Tests.Core.CommandHandlers;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Events;
using Intellias.CQRS.Tests.Core.Fakes;
using Intellias.CQRS.Tests.Tools;
using Moq;
using Xunit;

namespace Intellias.CQRS.Tests
{
    public class SelfProcessedBusTests
    {
        [Fact]
        public void SelfCommandBusTest()
        {
            var es = new InProcessEventStore();
            var eb = new Mock<IEventBus>();
            var sp = new Mock<IServiceProvider>();
            sp.Setup(x => x.GetService(typeof(DemoCommandHandlers))).Returns(new DemoCommandHandlers(es, eb.Object));
            var bus = new SelfProcessedCommandBus(new HandlerManager(new HandlerDependencyResolver(sp.Object, new List<Assembly> { typeof(DemoCommandHandlers).Assembly })));

            var result = bus.PublishAsync(new TestCreateCommand { AggregateRootId = "test" }).Result;
            Assert.True(result.Success);
        }

        [Fact]
        public void SelfEventBusTest()
        {
            var sp = new Mock<IServiceProvider>();
            var bus = new SelfProcessedEventBus(new HandlerManager(new HandlerDependencyResolver(sp.Object, new List<Assembly>())));

            var result = bus.PublishAsync(new TestCreatedEvent()).Result;
            Assert.True(result.Success);
        }

        [Fact]
        public void SelfCommandBusHandlersNotAddedFailTest()
        {
            var sp = new Mock<IServiceProvider>();

            // We add here assembly with command handler but not inject command handler to ServiceProvider
            var bus = new SelfProcessedCommandBus(new HandlerManager(new HandlerDependencyResolver(
                sp.Object,
                new[] { Assembly.GetExecutingAssembly() })));

            var result = bus.PublishAsync(new TestCreateCommand()).Result;
            Assert.False(result.Success);
        }

        [Fact]
        public void SelfEventBusHandlersNotAddedFailTest()
        {
            var sp = new Mock<IServiceProvider>();

            // We add here assembly with command handler but not inject command handler to ServiceProvider
            var bus = new SelfProcessedEventBus(new HandlerManager(new HandlerDependencyResolver(
                sp.Object,
                new[] { Assembly.GetExecutingAssembly() })));

            var result = bus.PublishAsync(new TestCreatedEvent()).Result;
            Assert.False(result.Success);
        }
    }
}
