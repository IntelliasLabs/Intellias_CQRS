using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;

namespace Intellias.CQRS.Core.Tests.Fakes
{
    internal class InProcessCommandBus : ICommandBus
    {
        public async Task<CommandResult> SendAsync<T>(T command) where T : ICommand
        {
            return await Task.FromResult(CommandResult.Success);
        }
    }
}
