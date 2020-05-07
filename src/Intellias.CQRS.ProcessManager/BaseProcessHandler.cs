using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.ProcessManager.Pipelines.Requests;
using Intellias.CQRS.ProcessManager.Pipelines.Response;

namespace Intellias.CQRS.ProcessManager
{
    /// <summary>
    /// Base process handler.
    /// </summary>
    public abstract class BaseProcessHandler
    {
        /// <summary>
        /// Create response.
        /// </summary>
        /// <typeparam name="TState">State type.</typeparam>
        /// <param name="processRequest">Process request.</param>
        /// <param name="messages">Messages.</param>
        /// <returns>Process response.</returns>
        protected ProcessResponse Response<TState>(ProcessRequest<TState> processRequest, params AbstractMessage[] messages)
            where TState : class
            => new ProcessResponse(processRequest.Id, messages.Select(m => new ProcessMessage(m)).ToArray());

        /// <summary>
        /// Create response async.
        /// </summary>
        /// <typeparam name="TState">State type.</typeparam>
        /// <param name="processRequest">Process request.</param>
        /// <param name="commands">Commands.</param>
        /// <returns>Task of process response.</returns>
        protected Task<ProcessResponse> ResponseAsync<TState>(ProcessRequest<TState> processRequest, params AbstractMessage[] commands)
            where TState : class
            => Task.FromResult(Response(processRequest, commands));

        /// <summary>
        /// Create empty response.
        /// </summary>
        /// <typeparam name="TState">State type.</typeparam>
        /// <param name="processRequest">Process request.</param>
        /// <returns>Empty response.</returns>
        protected ProcessResponse EmptyResponse<TState>(ProcessRequest<TState> processRequest)
            where TState : class
            => new ProcessResponse(processRequest.Id);

        /// <summary>
        /// Create empty response async.
        /// </summary>
        /// <typeparam name="TState">State type.</typeparam>
        /// <param name="processRequest">Process request.</param>
        /// <returns>Task of empty response.</returns>
        protected Task<ProcessResponse> EmptyResponseAsync<TState>(ProcessRequest<TState> processRequest)
            where TState : class
            => Task.FromResult(EmptyResponse(processRequest));
    }
}
