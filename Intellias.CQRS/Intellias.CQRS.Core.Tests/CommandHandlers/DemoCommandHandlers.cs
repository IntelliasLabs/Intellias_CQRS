using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Tests.Commands;

namespace Intellias.CQRS.Core.Tests.CommandHandlers
{
    /// <summary>
    /// Demo command handlers
    /// </summary>
    public class DemoCommandHandlers : ICommandHandler<DemoCreateCommand>
    {
        /// <summary>
        /// Handels create command
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ICommandResult> HandleAsync(DemoCreateCommand message)
        {
            return await Task.FromResult(CommandResult.Success);
        }
    }
}
