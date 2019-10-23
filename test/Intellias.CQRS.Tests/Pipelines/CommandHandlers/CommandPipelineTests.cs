using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;
using Intellias.CQRS.Pipelines.CommandHandlers.Behaviors;
using Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules;
using Intellias.CQRS.Tests.Core.Pipelines.Builders;
using Intellias.CQRS.Tests.Utils;
using Intellias.CQRS.Tests.Utils.Pipelines.Fakes;
using Xunit;

namespace Intellias.CQRS.Tests.Pipelines.CommandHandlers
{
    public class CommandPipelineTests
    {
        private readonly CommandHandlerHost host = new CommandHandlerHost();

        public CommandPipelineTests()
        {
            host.RegisterHandler<FakeCommandHandler, FakeCreateCommand>();
            host.RegisterHandler<FakeCommandHandler, FakeUpdateCommand>();
        }

        [Fact]
        public async Task Create_Always_PublishesCreatedIntegrationEvent()
        {
            var command = Fixtures.Pipelines.FakeCreateCommand();
            var result = await host.SendAsync(command);

            var expectedIntegrationEvent = Fixtures.IntegrationEvent<FakeCreatedIntegrationEvent>(command, e =>
            {
                e.Data = command.Data;
            });

            result.Should().BeOfType<IntegrationEventExecutionResult>()
                .Which.Event.Should().BeOfType<FakeCreatedIntegrationEvent>()
                .Which.Should().BeEquivalentTo(expectedIntegrationEvent, options => options.ForIntegrationEvent());
        }

        [Fact]
        public async Task Update_BeforeCreated_ReturnsError()
        {
            // Update aggregate.
            var updateCommand = Fixtures.Pipelines.FakeUpdateCommand();
            var updateResult = await host.SendAsync(updateCommand);

            // Ensure returns unhandled error.
            updateResult.Should().BeOfType<FailedResult>().Which.Code.Should().Be(CoreErrorCodes.ValidationFailed.Code);
            updateResult.Should().BeOfType<FailedResult>().Which.Details.Single()
              .Code.Should().Be(CoreErrorCodes.AggregateRootNotFound.Code);
        }

        [Fact]
        public async Task Update_AfterCreatedWithEmptyData_PublishesUpdatedIntegrationEvent()
        {
            // Create aggregate.
            var createCommand = Fixtures.Pipelines.FakeCreateCommand();
            var createResult = await host.SendAsync(createCommand);

            createResult.Should().BeOfType<IntegrationEventExecutionResult>();

            // Update aggregate.
            var updateCommand = Fixtures.Pipelines.FakeUpdateCommand(new CommandSeed<FakeUpdateCommand>
            {
                Setup = c => c.Data = string.Empty,
                AggregateRootId = createCommand.AggregateRootId
            });
            var updateResult = await host.SendAsync(updateCommand);

            // Ensure returns validation error.
            updateResult.Should().BeOfType<FailedResult>()
                .Which.Code.Should().Be(CoreErrorCodes.ValidationFailed.Code);
        }

        [Fact]
        public async Task Update_AfterCreatedWithData_PublishesUpdatedIntegrationEvent()
        {
            // Create aggregate.
            var createCommand = Fixtures.Pipelines.FakeCreateCommand();
            var createResult = await host.SendAsync(createCommand);

            createResult.Should().BeOfType<IntegrationEventExecutionResult>();

            // Update aggregate.
            var updateCommand = Fixtures.Pipelines.FakeUpdateCommand(new CommandSeed<FakeUpdateCommand>
            {
                AggregateRootId = createCommand.AggregateRootId
            });
            var updateResult = await host.SendAsync(updateCommand);

            // Ensure is updated correctly.
            var expectedIntegrationEvent = Fixtures.IntegrationEvent<FakeUpdatedIntegrationEvent>(updateCommand, e =>
            {
                e.Data = updateCommand.Data;
            });

            updateResult.Should().BeOfType<IntegrationEventExecutionResult>()
                .Which.Event.Should().BeOfType<FakeUpdatedIntegrationEvent>()
                .Which.Should().BeEquivalentTo(expectedIntegrationEvent, options => options.ForIntegrationEvent());
        }
    }
}