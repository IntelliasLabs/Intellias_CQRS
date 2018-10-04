using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Storage;
using Intellias.CQRS.Core.Tests.Commands;
using Intellias.CQRS.Core.Tests.Domain;

namespace Intellias.CQRS.Core.Tests.CommandHandlers
{
    /// <summary>
    /// Demo command handlers
    /// </summary>
    public class DemoCommandHandlers : ICommandHandler<DemoCreateCommand>
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
        public async Task<ICommandResult> HandleAsync(DemoCreateCommand message)
        {
            var ar = new DemoRoot(message);
            await storage.CreateAsync(ar);
            return await Task.FromResult(CommandResult.Success);
        }
    }
}
