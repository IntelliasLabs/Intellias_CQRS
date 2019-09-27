using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Pipelines.CommandHandlers;
using MediatR;

namespace Intellias.CQRS.Tests.Utils.Pipelines.Fakes
{
    public class FakeCommandHandler : BaseCommandHandler,
        IRequestHandler<CommandRequest<FakeCreateCommand>, IExecutionResult>,
        IRequestHandler<CommandRequest<FakeUpdateCommand>, IExecutionResult>
    {
        public Task<IExecutionResult> Handle(CommandRequest<FakeCreateCommand> request, CancellationToken cancellationToken)
        {
            var (command, context, scope) = request;

            var aggregate = scope.CreateAggregate<FakeAggregateRoot, FakeAggregateState>(command.AggregateRootId, context);
            aggregate.Create(command.Data);
            var integrationEvent = IntegrationEvent<FakeCreatedIntegrationEvent>(context, e =>
            {
                e.Data = command.Data;
            });

            return Task.FromResult(integrationEvent);
        }

        public async Task<IExecutionResult> Handle(CommandRequest<FakeUpdateCommand> request, CancellationToken cancellationToken)
        {
            var (command, context, scope) = request;

            var aggregate = await scope.FindAggregateAsync<FakeAggregateRoot, FakeAggregateState>(command.AggregateRootId, context);
            if (aggregate == null)
            {
                return AggregateNotFound<FakeAggregateRoot>(command.AggregateRootId);
            }

            var result = aggregate.Update(command.Data);
            if (!result.Success)
            {
                return result;
            }

            return IntegrationEvent<FakeUpdatedIntegrationEvent>(context, e =>
            {
                e.Data = command.Data;
            });
        }
    }
}