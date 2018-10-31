using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Storage;
using Intellias.CQRS.Core.Tests.Domain;
using Intellias.CQRS.Tests.Core.Commands;

namespace Intellias.CQRS.Core.Tests.CommandHandlers
{
    /// <summary>
    /// Demo command handlers
    /// </summary>
    public class DemoCommandHandlers : ICommandHandler<TestCreateCommand>,
        ICommandHandler<TestUpdateCommand>,
        ICommandHandler<TestDeleteCommand>
    {
        private readonly IAggregateStorage<DemoRoot> storage;
        private readonly IEventBus eventBus;

        /// <summary>
        /// Creates an instance of demo command handler
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="eventBus"></param>
        public DemoCommandHandlers(IAggregateStorage<DemoRoot> storage, IEventBus eventBus)
        {
            this.storage = storage;
            this.eventBus = eventBus;
        }

        /// <summary>
        /// Handels create command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<ICommandResult> HandleAsync(TestCreateCommand command)
        {
            var ar = new DemoRoot(command);

            foreach (var pendingEvent in ar.Events)
            {
                await eventBus.PublishAsync(pendingEvent);
            }

            await storage.CreateAsync(ar);

            return await Task.FromResult(CommandResult.Success);
        }

        /// <summary>
        /// Handle Test Update Command
        /// </summary>
        /// <param name="command">command</param>
        /// <returns>result</returns>
        public async Task<ICommandResult> HandleAsync(TestUpdateCommand command)
        {
            var root = await storage.GetAsync(command.AggregateRootId, command.ExpectedVersion);

            var result = root.Update(command);

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Handle Test Delete Command
        /// </summary>
        /// <param name="command">command</param>
        /// <returns>result</returns>
        public async Task<ICommandResult> HandleAsync(TestDeleteCommand command)
        {
            var root = await storage.GetAsync(command.AggregateRootId, command.ExpectedVersion);

            var result = root.Deactivate();

            return await Task.FromResult(result);
        }
    }
}
