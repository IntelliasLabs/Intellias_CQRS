using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <inheritdoc />
    public class HandlerWrapper : IHandler<IMessage, IExecutionResult>
    {
        private readonly Func<IMessage, Task<IExecutionResult>> handle;

        /// <summary>
        /// Constructs handler wrapper from func
        /// </summary>
        /// <param name="handle"></param>
        public HandlerWrapper(Func<IMessage, Task<IExecutionResult>> handle)
        {
            this.handle = handle;
        }

        /// <inheritdoc />
        public Task<IExecutionResult> HandleAsync(IMessage message)
        {
            return handle(message);
        }
    }
}
