using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Notifications;
using Intellias.CQRS.Messaging.AzureServiceBus.Commands;
using Intellias.CQRS.ProcessManager;
using Intellias.CQRS.ProcessManager.Pipelines.Response;
using Intellias.CQRS.ProcessManager.Stores;
using Intellias.CQRS.Tests.Core.EventHandlers.Tests;
using Intellias.CQRS.Tests.Core.Fakes;
using Intellias.CQRS.Tests.ProcessManager.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Intellias.CQRS.Tests.ProcessManager
{
    public class ProcessManagerFlowTests
    {
        private readonly Fixture fixture;
        private readonly string actorId;
        private readonly FakeCommandBus<DefaultCommandBusOptions> commandBus;
        private readonly ProcessHandlerExecutor executor;
        private readonly Dictionary<Type, Dictionary<string, List<ProcessMessage>>> handlerStore =
            new Dictionary<Type, Dictionary<string, List<ProcessMessage>>>();

        public ProcessManagerFlowTests()
        {
            fixture = new Fixture();
            actorId = Guid.NewGuid().ToString();
            commandBus = new FakeCommandBus<DefaultCommandBusOptions>();

            var serviceProvider = new ServiceCollection()
                .AddProcessHandlers(typeof(ProcessHandler1).Assembly, o => o.ActorId = actorId)
                .AddSingleton(handlerStore)
                .AddSingleton(typeof(IProcessStore<>), typeof(FakeProcessStore<>))
                .AddSingleton<ICommandBus<DefaultCommandBusOptions>>(commandBus)
                .AddSingleton<INotificationBus, FakeNotificationBus>()
                .BuildServiceProvider();

            executor = serviceProvider.GetService<ProcessHandlerExecutor>();
        }

        [Fact]
        public async Task Process_IntegrationEvent_CommandPublished()
        {
            var @event = fixture.Build<FakeCreatedIntegrationEvent>()
                .With(s => s.IsReplay, false)
                .Create();

            await executor.ExecuteAsync<ProcessHandler1>(@event);

            var processCommands = handlerStore[typeof(ProcessHandler1)][@event.Id];
            processCommands.Should()
                .ContainSingle()
                .Which.IsPublished.Should().BeTrue();

            var publishedCommands = commandBus.PublishedCommands;
            publishedCommands.Should().BeEquivalentTo(processCommands
                .Select(s => s.Message)
                .OfType<object>()
                .ToArray());
        }

        [Fact]
        public async Task Process_ReplayedIntegrationEvent_CommandPublished()
        {
            var @event = fixture.Build<FakeCreatedIntegrationEvent>()
                .With(s => s.IsReplay, true)
                .Create();

            await executor.ExecuteAsync<ProcessHandler1>(@event);

            var isRequestProcessed = handlerStore.TryGetValue(typeof(ProcessHandler1), out var _);
            isRequestProcessed.Should().BeFalse();

            var publishedCommands = commandBus.PublishedCommands;
            publishedCommands.Should().BeEmpty();
        }

        [Fact]
        public async Task Process_RetriedIntegrationEvent_CommandPublished()
        {
            var @event = fixture.Build<FakeCreatedIntegrationEvent>()
                .With(s => s.IsReplay, false)
                .Create();

            await executor.ExecuteAsync<ProcessHandler1>(@event);
            await executor.ExecuteAsync<ProcessHandler1>(@event);

            var processCommands = handlerStore[typeof(ProcessHandler1)][@event.Id];
            processCommands.Should()
                .ContainSingle()
                .Which.IsPublished.Should().BeTrue();

            var publishedCommands = commandBus.PublishedCommands;
            publishedCommands.Should().BeEquivalentTo(processCommands
                .Select(s => s.Message)
                .OfType<object>()
                .ToArray());
        }

        [Fact]
        public async Task Process_QueryModel_CommandPublished()
        {
            var queryModel = fixture.Create<FakeSnapshotQueryModel>();

            await executor.ExecuteAsync<ProcessHandler2, FakeSnapshotQueryModel>(queryModel, s => s.First);

            var processCommands = handlerStore[typeof(ProcessHandler2)][$"{queryModel.First.EntryId}-{queryModel.First.EntryVersion}"];
            processCommands.Should()
                .ContainSingle()
                .Which.IsPublished.Should().BeTrue();

            var publishedCommands = commandBus.PublishedCommands;
            publishedCommands.Should().BeEquivalentTo(processCommands
                .Select(s => s.Message)
                .OfType<object>()
                .ToArray());
        }

        [Fact]
        public async Task Process_CustomState_CommandPublished()
        {
            var state = fixture.Create<CustomState>();

            await executor.ExecuteAsync<ProcessHandler2, CustomState>(state, s => s.Id);

            var processCommands = handlerStore[typeof(ProcessHandler2)][state.Id];
            processCommands.Should()
                .ContainSingle()
                .Which.IsPublished.Should().BeTrue();

            var publishedCommands = commandBus.PublishedCommands;
            publishedCommands.Should().BeEquivalentTo(processCommands
                .Select(s => s.Message)
                .OfType<object>()
                .ToArray());
        }

        [Fact]
        public async Task Process_TwoStateHandlers_CommandsPublished()
        {
            var @event = fixture.Build<FakeUpdatedIntegrationEvent>()
                .With(s => s.IsReplay, false)
                .Create();

            await executor.ExecuteAsync<ProcessHandler1>(@event);
            await executor.ExecuteAsync<ProcessHandler2>(@event);

            var processCommands = handlerStore[typeof(ProcessHandler1)][@event.Id]
                .Union(handlerStore[typeof(ProcessHandler2)][@event.Id]);

            var publishedCommands = commandBus.PublishedCommands;
            publishedCommands.Should().BeEquivalentTo(processCommands
                .Select(s => s.Message)
                .OfType<object>()
                .ToArray());
        }

        [Fact]
        public async Task Process_CustomActorIdIsSpecified_CustomActorIdIsSet()
        {
            var @event = fixture.Build<FakeCreatedIntegrationEvent>()
                .With(s => s.IsReplay, false)
                .Create();

            await executor.ExecuteAsync<ProcessHandler1>(@event);

            var processCommands = handlerStore[typeof(ProcessHandler1)][@event.Id];
            processCommands.Should()
                .ContainSingle()
                .Which.Message.Actor.UserId.Should().Be(actorId);
        }
    }
}