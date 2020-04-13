using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.ProcessManager.Pipelines.Requests;
using Intellias.CQRS.ProcessManager.Pipelines.Response;

namespace Intellias.CQRS.ProcessManager.Pipelines.Middlewares
{
    /// <summary>
    /// Process middleware delegate.
    /// </summary>
    /// <returns>Process response.</returns>
    public delegate Task<ProcessResponse> ProcessMiddlewareDelegate();

    /// <summary>
    /// Process middleware.
    /// </summary>
    /// <typeparam name="TState">Pipeline state.</typeparam>
    /// <typeparam name="TProcessHandler">Process handler type.</typeparam>
    public interface IProcessMiddleware<TState, TProcessHandler>
        where TState : class
        where TProcessHandler : BaseProcessHandler
    {
        /// <summary>
        /// Execute Pipeline.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="next">Next pipeline.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Process response.</returns>
        Task<ProcessResponse> Execute(ProcessRequest<TState> request, ProcessMiddlewareDelegate next, CancellationToken token);
    }
}
