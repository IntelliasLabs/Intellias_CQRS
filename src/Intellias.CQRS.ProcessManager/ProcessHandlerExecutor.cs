using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Queries.Mutable;
using Intellias.CQRS.ProcessManager.Pipelines;
using Intellias.CQRS.ProcessManager.Pipelines.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace Intellias.CQRS.ProcessManager
{
    /// <summary>
    /// Process manager executor.
    /// </summary>
    public class ProcessHandlerExecutor
    {
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessHandlerExecutor"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider.</param>
        public ProcessHandlerExecutor(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Execute process manager.
        /// </summary>
        /// <param name="state">Query model state.</param>
        /// <param name="getId">Get id.</param>
        /// <typeparam name="TProcessHandler">Process handler.</typeparam>
        /// <typeparam name="TState">Process manager state.</typeparam>
        /// <returns>Task.</returns>
        public Task ExecuteAsync<TProcessHandler, TState>(TState state, Func<TState, SnapshotId> getId)
            where TState : BaseMutableQueryModel, new()
            where TProcessHandler : BaseProcessHandler, IProcessHandler<TState>
        {
            return ExecuteAsync<TProcessHandler, TState>(new ProcessRequest<TState>(state, getId));
        }

        /// <summary>
        /// Execute process manager.
        /// </summary>
        /// <typeparam name="TProcessHandler">Process handler.</typeparam>
        /// <param name="event">Integration event.</param>
        /// <returns>Task.</returns>
        public Task ExecuteAsync<TProcessHandler>(IntegrationEvent @event)
            where TProcessHandler : BaseProcessHandler
        {
            var stateType = @event.GetType();
            var requestType = typeof(ProcessRequest<>).MakeGenericType(stateType);
            var request = Activator.CreateInstance(requestType, new object[] { @event });

            var method = GetType()
                .GetMethod(nameof(ExecuteAsync), BindingFlags.NonPublic | BindingFlags.Instance);
            if (method is null)
            {
                throw new MissingMethodException(nameof(ExecuteAsync));
            }

            return method
                .MakeGenericMethod(typeof(TProcessHandler), stateType)
                .Invoke(this, new[] { request }) as Task;
        }

        /// <summary>
        /// Execute process handler.
        /// </summary>
        /// <param name="request">Process request.</param>
        /// <typeparam name="TProcessHandler">Process handler.</typeparam>
        /// <typeparam name="TState">Process manager state.</typeparam>
        /// <returns>Task.</returns>
        private Task ExecuteAsync<TProcessHandler, TState>(ProcessRequest<TState> request)
            where TState : class
            where TProcessHandler : BaseProcessHandler
        {
            var instance = serviceProvider.GetService<TProcessHandler>();
            if (instance is IProcessHandler<TState> handler)
            {
                var piplineExecutor = serviceProvider.GetService<ProcessPipelineExecutor<TState>>();
                return piplineExecutor.ExecutePipline(request, handler, CancellationToken.None);
            }

            return Task.CompletedTask;
        }
    }
}
