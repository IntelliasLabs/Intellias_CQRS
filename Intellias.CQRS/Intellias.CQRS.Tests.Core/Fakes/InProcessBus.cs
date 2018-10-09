using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <inheritdoc />
    public class InProcessBus : IMessageBus<IMessage, IExecutionResult>
    {
        private readonly IEnumerable<IHandler<IMessage, IExecutionResult>> handlers;

        /// <summary>
        /// Creates message bus
        /// </summary>
        /// <param name="handlers"></param>
        public InProcessBus(params IHandler<IMessage, IExecutionResult>[] handlers)
        {
            this.handlers = handlers;
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> PublishAsync(IMessage msg)
        {
            var results = new List<IExecutionResult>();
            foreach (var handler in handlers)
            {
                var result = await handler.HandleAsync(msg).ConfigureAwait(false);
                results.Add(result);
            }

            // Command result
            if (results.Count == 1)
            {
                return await Task.FromResult(results.Single()).ConfigureAwait(false);
            }

            return await Task.FromResult(CommandResult.Success).ConfigureAwait(false);
        }
    }
}
