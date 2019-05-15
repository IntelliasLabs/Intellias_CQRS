using System;
using System.Collections.Generic;
using System.Reflection;
using Intellias.CQRS.Core.Tools;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Events;
using Moq;
using Xunit;

namespace Intellias.CQRS.Tests
{
    public class SelfProcessedBusTests
    {
        [Fact]
        public void SelfCommandBusTest()
        {
            var sp = new Mock<IServiceProvider>();
            var bus = new SelfProcessedCommandBus(new HandlerManager(new HandlerDependencyResolver(sp.Object, new List<Assembly>())));

            var result = bus.PublishAsync(new TestCreateCommand()).Result;
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void SelfEventBusTest()
        {
            var sp = new Mock<IServiceProvider>();
            var bus = new SelfProcessedEventBus(new HandlerManager(new HandlerDependencyResolver(sp.Object, new List<Assembly>())));

            var result = bus.PublishAsync(new TestCreatedEvent()).Result;
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void SelfCommandBusHandlersNotAddedFailTest()
        {
            var sp = new Mock<IServiceProvider>();

            // We add here assembly with command handler but not inject command handler to ServiceProvider
            var bus = new SelfProcessedCommandBus(new HandlerManager(new HandlerDependencyResolver(sp.Object,
                new[] { Assembly.GetExecutingAssembly() })));

            var result = bus.PublishAsync(new TestCreateCommand()).Result;
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void SelfEventBusHandlersNotAddedFailTest()
        {
            var sp = new Mock<IServiceProvider>();

            // We add here assembly with command handler but not inject command handler to ServiceProvider
            var bus = new SelfProcessedEventBus(new HandlerManager(new HandlerDependencyResolver(sp.Object,
                new[] { Assembly.GetExecutingAssembly() })));

            var result = bus.PublishAsync(new TestCreatedEvent()).Result;
            Assert.False(result.IsSuccess);
        }
    }
}
