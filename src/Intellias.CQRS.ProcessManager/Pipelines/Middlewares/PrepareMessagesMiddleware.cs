using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.ProcessManager.Pipelines.Requests;
using Intellias.CQRS.ProcessManager.Pipelines.Response;
using Intellias.CQRS.ProcessManager.Stores;

namespace Intellias.CQRS.ProcessManager.Pipelines.Middlewares
{
    /// <summary>
    /// Prepare messages middleware.
    /// </summary>
    /// <typeparam name="TState">State of request.</typeparam>
    /// <typeparam name="TProcessHandler">Process handler type.</typeparam>
    public class PrepareMessagesMiddleware<TState, TProcessHandler> : IProcessMiddleware<TState, TProcessHandler>
        where TState : class
        where TProcessHandler : BaseProcessHandler
    {
        private readonly IProcessStore<TProcessHandler> store;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrepareMessagesMiddleware{TState, TProcessHandler}"/> class.
        /// </summary>
        /// <param name="store">Process manager store.</param>
        public PrepareMessagesMiddleware(IProcessStore<TProcessHandler> store)
        {
            this.store = store;
        }

        /// <inheritdoc/>
        public async Task<ProcessResponse> Execute(ProcessRequest<TState> request, ProcessMiddlewareDelegate next, CancellationToken token)
        {
            var messages = await store.GetMessagesAsync(request.Id);

            return messages.Count > 0
                ? new ProcessResponse(request.Id, messages, true)
                : await next();
        }
    }
}