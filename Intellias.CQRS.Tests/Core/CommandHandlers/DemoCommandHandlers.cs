using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Domain;

namespace Intellias.CQRS.Tests.Core.CommandHandlers
{
    /// <summary>
    /// Demo command handlers.
    /// </summary>
    public class DemoCommandHandlers : AbstractCommandHandler,
        ICommandHandler<TestCreateCommand>,
        ICommandHandler<TestUpdateCommand>,
        ICommandHandler<TestDeleteCommand>
    {
        public DemoCommandHandlers(IEventStore store, IEventBus bus)
            : base(store, bus)
        {
        }

        public async Task<IExecutionResult> HandleAsync(TestCreateCommand command)
        {
            var ar = new TestRoot(command);

            var processedEvents = await Store.SaveAsync(ar);
            await Task.WhenAll(processedEvents.Select(Bus.PublishAsync));

            return await Task.FromResult(new SuccessfulResult());
        }

        public async Task<IExecutionResult> HandleAsync(TestUpdateCommand command)
        {
            var ar = await GetAggregateAsync<TestRoot, TestState>(command.AggregateRootId);
            var result = ar.Update(command);

            var processedEvents = await Store.SaveAsync(ar);
            await Task.WhenAll(processedEvents.Select(Bus.PublishAsync));

            return await Task.FromResult(result);
        }

        public async Task<IExecutionResult> HandleAsync(TestDeleteCommand command)
        {
            var ar = await GetAggregateAsync<TestRoot, TestState>(command.AggregateRootId);
            var result = ar.Deactivate();

            var processedEvents = await Store.SaveAsync(ar);
            await Task.WhenAll(processedEvents.Select(Bus.PublishAsync));

            return await Task.FromResult(result);
        }
    }
}
