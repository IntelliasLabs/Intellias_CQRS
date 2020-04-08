using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.ProcessManager.Pipelines.Requests;
using Intellias.CQRS.ProcessManager.Pipelines.Response;

namespace Intellias.CQRS.ProcessManager
{
    /// <summary>
    /// Process handler.
    /// </summary>
    /// <typeparam name="TState">Request state.</typeparam>
    public interface IProcessHandler<TState>
        where TState : class
    {
        /// <summary>
        /// Handler process request.
        /// </summary>
        /// <param name="request">Process request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Process response.</returns>
        Task<ProcessResponse> Handle(ProcessRequest<TState> request, CancellationToken cancellationToken);
    }
}
