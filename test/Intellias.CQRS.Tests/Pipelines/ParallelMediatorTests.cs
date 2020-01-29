using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Pipelines;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Intellias.CQRS.Tests.Pipelines
{
    public class ParallelMediatorTests
    {
        [Fact]
        public async Task Publish_Always_ExecutesAllNotificationHandlers()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<INotificationHandler<FakeNotification>, FakeNotificationHandler1>()
                .AddTransient<INotificationHandler<FakeNotification>, FakeNotificationHandler2>()
                .BuildServiceProvider();

            var mediator = new ParallelMediator(t => serviceProvider.GetService(t));
            var notification = new FakeNotification();

            await mediator.Publish(notification);

            notification.TriggeredHandlers.Should().BeEquivalentTo(typeof(FakeNotificationHandler1), typeof(FakeNotificationHandler2));
        }

        private class FakeNotification : INotification
        {
            public List<Type> TriggeredHandlers { get; } = new List<Type>();

            public void Triggered(Type handlerType)
            {
                TriggeredHandlers.Add(handlerType);
            }
        }

        private class FakeNotificationHandler1 : INotificationHandler<FakeNotification>
        {
            public Task Handle(FakeNotification notification, CancellationToken cancellationToken)
            {
                notification.Triggered(this.GetType());
                return Task.CompletedTask;
            }
        }

        private class FakeNotificationHandler2 : INotificationHandler<FakeNotification>
        {
            public Task Handle(FakeNotification notification, CancellationToken cancellationToken)
            {
                notification.Triggered(this.GetType());
                return Task.CompletedTask;
            }
        }
    }
}