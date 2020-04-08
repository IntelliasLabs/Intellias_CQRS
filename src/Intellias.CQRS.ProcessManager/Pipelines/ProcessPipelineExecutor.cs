using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.ProcessManager.Pipelines.Middlewares;
using Intellias.CQRS.ProcessManager.Pipelines.Requests;

namespace Intellias.CQRS.ProcessManager.Pipelines
{
    /// <summary>
    /// Process pipeline executor.
    /// </summary>
    /// <typeparam name="TState">Process state.</typeparam>
    public class ProcessPipelineExecutor<TState>
        where TState : class
    {
        private readonly IEnumerable<IProcessMiddleware<TState>> middlewares;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessPipelineExecutor{TState}"/> class.
        /// </summary>
        /// <param name="middlewares">Middlewares.</param>
        public ProcessPipelineExecutor(IEnumerable<IProcessMiddleware<TState>> middlewares)
        {
            this.middlewares = middlewares;
        }

        /// <summary>
        /// Execute process pipeline.
        /// </summary>
        /// <param name="request">Process request.</param>
        /// <param name="handler">Process handler.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Task.</returns>
        public Task ExecutePipline(ProcessRequest<TState> request, IProcessHandler<TState> handler, CancellationToken token)
        {
            var delegates = new List<Func<ProcessMiddlewareDelegate, ProcessMiddlewareDelegate>>();
            foreach (var middleware in middlewares)
            {
                delegates.Add(next =>
                {
                    return () => middleware.Execute(request, next, token);
                });
            }

            ProcessMiddlewareDelegate execute = () => handler.Handle(request, token);
            foreach (var @delegate in delegates)
            {
                execute = @delegate(execute);
            }

            return execute();
        }
    }
}
