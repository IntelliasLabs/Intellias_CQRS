using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Tests.Fakes
{
    internal class HandlerWrapper : IHandler<IMessage, IExecutionResult>
    {
        private readonly Func<IMessage, Task<IExecutionResult>> handle;

        public HandlerWrapper(Func<IMessage, Task<IExecutionResult>> handle)
        {
            this.handle = handle;
        }

        public async Task<IExecutionResult> HandleAsync(IMessage message)
        {
            var result = await handle(message);
            return await Task.FromResult(result);
        }
    }
}
