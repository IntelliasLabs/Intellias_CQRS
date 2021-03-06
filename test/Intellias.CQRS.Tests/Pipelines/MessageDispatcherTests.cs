using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Pipelines;
using Intellias.CQRS.Pipelines.CommandHandlers;
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
        private readonly FakeDispatcherCommandHandler commandHandler;
        private readonly FakeDispatcherEventHandler eventHandler;

        public MessageDispatcherTests()
        {
            commandHandler = new FakeDispatcherCommandHandler();
            eventHandler = new FakeDispatcherEventHandler();

            var services = new ServiceCollection()
                .AddMediatR(typeof(MessageDispatcherTests))
                .AddSingleton<IRequestHandler<CommandRequest<FakeDispatcherCommand>, IExecutionResult>>(commandHandler)
                .AddSingleton<INotificationHandler<IntegrationEventNotification<FakeDispatcherEvent>>>(eventHandler);

            var mediator = services
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();

            dispatcher = new MessageDispatcher(mediator);
        }

        [Fact]
        public async Task DispatchCommand_HasHandler_DispatchesToCommandHandler()
        {
            var command = Fixtures.Pipelines.FakeDispatcherCommand();

            await dispatcher.DispatchCommandAsync(command);
            var dispatchedCommand = commandHandler.DispatchedMessages.Single();

            dispatchedCommand.Should().BeEquivalentTo(command);
        }

        [Fact]
        public async Task DispatchCommand_NoHandler_Throws()
        {
            var command = new NoHandlerCommand();

            await dispatcher.Awaiting(d => d.DispatchCommandAsync(command)).Should()
                .ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task DispatchCommand_CommandIsNotForMediatR_DispatchesToCommandHandler()
        {
            var command = new InvalidCommand();

            await dispatcher.Awaiting(d => d.DispatchCommandAsync(command)).Should()
                .ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task DispatchEvent_HasHandler_DispatchesToEventHandler()
        {
            var @event = Fixtures.Pipelines.FakeDispatcherEvent();

            await dispatcher.DispatchEventAsync(@event);
            var dispatchedEvent = eventHandler.DispatchedMessages.Single();

            dispatchedEvent.Should().BeEquivalentTo(@event);
        }

        [Fact]
        public async Task DispatchEvent_NoHandler_NoExceptions()
        {
            var @event = new NoHandlerEvent();

            await dispatcher.Awaiting(d => d.DispatchEventAsync(@event)).Should()
                .NotThrowAsync();
        }

        private class NoHandlerCommand : Command, IRequest<IExecutionResult>
        {
        }

        private class InvalidCommand : Command
        {
        }

        private class NoHandlerEvent : IntegrationEvent
        {
        }

        private class FakeDispatcherCommandHandler : IRequestHandler<CommandRequest<FakeDispatcherCommand>, IExecutionResult>
        {
            private readonly List<FakeDispatcherCommand> dispatchedMessages = new List<FakeDispatcherCommand>();

            public IReadOnlyList<FakeDispatcherCommand> DispatchedMessages => dispatchedMessages;

            public Task<IExecutionResult> Handle(CommandRequest<FakeDispatcherCommand> request, CancellationToken cancellationToken)
            {
                dispatchedMessages.Add(request.Command);
                return Task.FromResult((IExecutionResult)new SuccessfulResult());
            }
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