using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Pipelines;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using Intellias.CQRS.Tests.Utils;
using Intellias.CQRS.Tests.Utils.Pipelines.Fakes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Intellias.CQRS.Tests.Pipelines
{
    public class MessageDispatcherTests
    {
        private readonly MessageDispatcher dispatcher;
        private readonly FakeDispatcherEventHandler eventHandler;

        public MessageDispatcherTests()
        {
            eventHandler = new FakeDispatcherEventHandler();

            var services = new ServiceCollection()
                .AddMediatR(typeof(MessageDispatcherTests))
                .AddSingleton<INotificationHandler<IntegrationEventNotification<FakeDispatcherEvent>>>(eventHandler);

            var mediator = services
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();

            dispatcher = new MessageDispatcher(mediator);
        }

        [Fact]
        public async Task DispatchEvent_HasHandler_DispatchesToEventHandler()
        {
            var @event = Fixtures.Pipelines.FakeDispatcherEvent();
            var serializedMessage = @event.ToJson();

            await dispatcher.DispatchEventAsync(serializedMessage);
            var dispatchedEvent = eventHandler.DispatchedMessages.Single();

            dispatchedEvent.Should().BeEquivalentTo(@event);
        }

        [Fact]
        public async Task DispatchEvent_NotTypeName_Throws()
        {
            var @event = new { a = 1 };
            var serializedMessage = @event.ToJson();

            await dispatcher.Awaiting(d => d.DispatchEventAsync(serializedMessage)).Should()
                .ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task DispatchEvent_NotIMessage_Throws()
        {
            var @event = new { a = 1, typeName = typeof(object) };
            var serializedMessage = @event.ToJson();

            await dispatcher.Awaiting(d => d.DispatchEventAsync(serializedMessage)).Should()
                .ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task DispatchEvent_NoHandler_NoExceptions()
        {
            var @event = new NoHandlerEvent();
            var serializedMessage = @event.ToJson();

            await dispatcher.Awaiting(d => d.DispatchEventAsync(serializedMessage)).Should()
                .NotThrowAsync();
        }

        private class NoHandlerEvent : IntegrationEvent
        {
        }

        private class FakeDispatcherEventHandler : INotificationHandler<IntegrationEventNotification<FakeDispatcherEvent>>
        {
            private readonly List<FakeDispatcherEvent> dispatchedMessages = new List<FakeDispatcherEvent>();

            public IReadOnlyList<FakeDispatcherEvent> DispatchedMessages => dispatchedMessages;

            public Task Handle(IntegrationEventNotification<FakeDispatcherEvent> notification, CancellationToken cancellationToken)
            {
                dispatchedMessages.Add(notification.IntegrationEvent);
                return Task.CompletedTask;
            }
        }
    }
}