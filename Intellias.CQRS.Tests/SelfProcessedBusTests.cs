using System;
using System.Collections.Generic;
using System.Reflection;
using Intellias.CQRS.Core.Tools;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Events;
using Intellias.CQRS.Tests.Core.Fakes;
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
    }
}
