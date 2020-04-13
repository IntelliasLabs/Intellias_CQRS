using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.ProcessManager.Pipelines.Requests;
using Intellias.CQRS.ProcessManager.Pipelines.Response;
using Intellias.CQRS.ProcessManager.Stores;

namespace Intellias.CQRS.ProcessManager.Pipelines.Middlewares
{
    /// <summary>
    /// Persist messages middleware.
    /// </summary>
    /// <typeparam name="TState">State of request.</typeparam>
    /// <typeparam name="TProcessHandler">Process handler type.</typeparam>
    public class PersistMessagesMiddleware<TState, TProcessHandler> : IProcessMiddleware<TState, TProcessHandler>
        where TState : class
        where TProcessHandler : BaseProcessHandler
    {
        private readonly IProcessStore<TProcessHandler> store;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistMessagesMiddleware{TState, TProcessHandler}"/> class.
        /// </summary>
        /// <param name="store">Process manager store.</param>
        public PersistMessagesMiddleware(IProcessStore<TProcessHandler> store)
        {
            this.store = store;
        }

        /// <inheritdoc/>
        public async Task<ProcessResponse> Execute(ProcessRequest<TState> request, ProcessMiddlewareDelegate next, CancellationToken token)
        {
            var response = await next();
            if (!response.IsPersisted && response.ProcessMessages.Count > 0)
            {
                await store.PersistMessagesAsync(request.Id, response.ProcessMessages.Select(s => s.Message).ToArray());
            }

            return new ProcessResponse(response.Id, response.ProcessMessages, true);
        }
    }
}
