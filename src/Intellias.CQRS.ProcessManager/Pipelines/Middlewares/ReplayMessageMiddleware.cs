using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.ProcessManager.Pipelines.Requests;
using Intellias.CQRS.ProcessManager.Pipelines.Response;

namespace Intellias.CQRS.ProcessManager.Pipelines.Middlewares
{
    /// <summary>
    /// Replay message middleware.
    /// </summary>
    /// <typeparam name="TState">Process request state.</typeparam>
    public class ReplayMessageMiddleware<TState> : IProcessMiddleware<TState>
        where TState : class
    {
        /// <inheritdoc/>
        public async Task<ProcessResponse> Execute(ProcessRequest<TState> request, ProcessMiddlewareDelegate next, CancellationToken token)
        {
            return request.IsReplay
                ? new ProcessResponse(request.Id)
                : await next();
        }
    }
}
