using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <inheritdoc />
    public class HandlerWrapper<T, TR> : IHandler<IMessage, IExecutionResult> where T :IMessage where TR : IExecutionResult
    {
        private readonly IHandler<T, TR> handler;

        /// <summary>
        /// Constructs handler wrapper from func
        /// </summary>
        /// <param name="handler"></param>
        public HandlerWrapper(IHandler<T, TR> handler)
        {
            this.handler = handler;
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> HandleAsync(IMessage message)
        {
            return await handler.HandleAsync((T)message);
        }
    }
}
