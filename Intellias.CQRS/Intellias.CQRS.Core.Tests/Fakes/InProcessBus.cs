using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Tests.Fakes
{
    internal class InProcessBus : IMessageBus<IMessage, IExecutionResult>
    {
        private readonly IEnumerable<IHandler<IMessage, IExecutionResult>> handlers;

        public InProcessBus(params IHandler<IMessage, IExecutionResult>[] handlers)
        {
            this.handlers = handlers;
        }

        public async Task<IExecutionResult> PublishAsync(IMessage msg)
        {
            var results = new List<IExecutionResult>();
            foreach (var handler in handlers)
            {
                var result = await handler.HandleAsync(msg);
                results.Add(result);
            }

            // Command result
            if (results.Count == 1)
            {
                return await Task.FromResult(results[1]);
            }

            return await Task.FromResult(CommandResult.Success);
        }
    }
}
