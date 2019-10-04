using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Pipelines.CommandHandlers;
using Intellias.CQRS.Tests.Core.Fakes;
using Intellias.CQRS.Tests.Core.Pipelines.CommandHandlers;
using Intellias.CQRS.Tests.Utils;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Intellias.CQRS.Tests.Pipelines.CommandHandlers
{
    public class SubdomainTestFlowTests
    {
        private readonly FakeMediator mediator = new FakeMediator();
        private readonly SubdomainTestHost testHost;

        public SubdomainTestFlowTests()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IMediator>(mediator)
                .BuildServiceProvider();

            testHost = new SubdomainTestHost(serviceProvider);
        }

        [Fact]
        public async Task SubdomainTestFlow_ExecutionSucceed_UpdatesContext()
        {
            mediator.SetupRequestHandler<CommandRequest<FakeCommand>, IExecutionResult>(request => new SuccessfulResult());

            var step = new FakeCommandStep();

            var (result, context) = await testHost.CreateFlow()
                .With(step)
                .RunAsync();

            result.Should().BeOfType<SuccessfulResult>();
            context.ExecutedCommands.Single().Should().BeOfType<FakeCommand>()
                .Which.Key.Should().Be(step.CommandKey);
            context.ExpectedEvents.Single().Should().BeOfType<FakeIntegrationEvent>()
                .Which.Key.Should().Be(step.EventKey);
        }

        [Fact]
        public async Task SubdomainTestFlow_ExecutionFailed_StopsExecution()
        {
            mediator.SetupRequestHandler<CommandRequest<FakeCommand>, IExecutionResult>(request => new FailedResult("Something bad happened."));

            var step1 = new FakeCommandStep();
            var step2 = new FakeCommandStep();

            var (result, context) = await testHost.CreateFlow()
                .With(step1)
                .With(step2)
                .RunAsync();

            result.Should().BeOfType<FailedResult>();
            context.ExecutedCommands.Should().HaveCount(1);
            context.ExpectedEvents.Should().HaveCount(1);
        }

        [Fact]
        public async Task SubdomainTestFlow_EmptyStepResult_DoesntUpdateContext()
        {
            var (_, context) = await testHost.CreateFlow()
                .With(new FakeEmptyStep())
                .RunAsync();

            context.ExecutedCommands.Should().BeEmpty();
            context.ExpectedEvents.Should().BeEmpty();
        }

        private class FakeEmptyStep : ITestFlowStep
        {
            public ITestFlowStepResult Execute(TestFlowExecutionContext context)
            {
                return new EmptyStepResult();
            }
        }

        private class FakeCommandStep : ITestFlowStep
        {
            public string CommandKey { get; } = FixtureUtils.String();

            public string EventKey { get; } = FixtureUtils.String();

            public ITestFlowStepResult Execute(TestFlowExecutionContext context)
            {
                return new CommandStepResult(new FakeCommand(CommandKey), new FakeIntegrationEvent(EventKey));
            }
        }

        private class FakeCommand : Command
        {
            public FakeCommand(string key)
            {
                Key = key;
                Metadata[MetadataKey.UserId] = Guid.NewGuid().ToString();
            }

            public string Key { get; }
        }

        private class FakeIntegrationEvent : IntegrationEvent
        {
            public FakeIntegrationEvent(string key)
            {
                Key = key;
            }

            public string Key { get; }
        }
    }
}