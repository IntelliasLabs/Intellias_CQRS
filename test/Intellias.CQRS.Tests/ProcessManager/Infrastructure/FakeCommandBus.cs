using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Tests.ProcessManager.Infrastructure
{
    public class FakeCommandBus<TCommandBusOptions> : ICommandBus<TCommandBusOptions>
        where TCommandBusOptions : ICommandBusOptions
    {
        private readonly List<ICommand> store = new List<ICommand>();

        public IReadOnlyCollection<ICommand> PublishedCommands => store.AsReadOnly();

        public Task<IExecutionResult> PublishAsync(ICommand cmd)
        {
            store.Add(cmd);

            return Task.FromResult<IExecutionResult>(new SuccessfulResult());
        }
    }
}
