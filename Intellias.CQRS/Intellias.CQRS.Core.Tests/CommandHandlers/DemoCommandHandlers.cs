using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
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

        /// <summary>
        /// Creates an instance of demo command handler
        /// </summary>
        /// <param name="storage"></param>
        public DemoCommandHandlers(IAggregateStorage<DemoRoot> storage)
        {
            this.storage = storage;
        }

        /// <summary>
        /// Handels create command
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ICommandResult> HandleAsync(TestCreateCommand message)
        {
            var ar = new DemoRoot(message);
            
            await storage.CreateAsync(ar);

            return await Task.FromResult(CommandResult.Success);
        }

        /// <summary>
        /// Handle Test Update Command
        /// </summary>
        /// <param name="message">command</param>
        /// <returns>result</returns>
        public async Task<ICommandResult> HandleAsync(TestUpdateCommand message)
        {
            var root = await storage.GetAsync(message.AggregateRootId, message.ExpectedVersion);

            var result = root.Update(message);

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Handle Test Delete Command
        /// </summary>
        /// <param name="message">command</param>
        /// <returns>result</returns>
        public async Task<ICommandResult> HandleAsync(TestDeleteCommand message)
        {
            var root = await storage.GetAsync(message.AggregateRootId, message.ExpectedVersion);

            var result = root.Deactivate();

            return await Task.FromResult(result);
        }
    }
}
