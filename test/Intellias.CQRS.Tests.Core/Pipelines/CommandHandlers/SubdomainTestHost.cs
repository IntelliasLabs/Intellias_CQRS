using System;
using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Pipelines.CommandHandlers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Intellias.CQRS.Tests.Core.Pipelines.CommandHandlers
{
    /// <summary>
    /// Host for running subdomain test flows.
    /// </summary>
    public class SubdomainTestHost
    {
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubdomainTestHost"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider with subdomain services.</param>
        public SubdomainTestHost(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Sends command to subdomain.
        /// </summary>
        /// <param name="command">Command to be handled.</param>
        /// <returns>Execution result.</returns>
        public Task<IExecutionResult> SendAsync(Command command)
        {
            // Restore real command type to have MediatR handle it right.
            var commandType = command.GetType();
            var commandRequestType = typeof(CommandRequest<>).MakeGenericType(commandType);
            var commandRequest = Activator.CreateInstance(commandRequestType, command);

            // Execute MediatR pipeline.
            var mediator = this.serviceProvider.GetRequiredService<IMediator>();
            var sendMethodInfo = mediator.GetType().GetMethod(nameof(IMediator.Send));
            if (sendMethodInfo == null)
            {
                throw new InvalidOperationException($"Unable to resolve '{nameof(IMediator.Send)}' from '{mediator.GetType()}'.");
            }

            var sendGenericMethodInfo = sendMethodInfo.MakeGenericMethod(typeof(IExecutionResult));
            var result = (Task<IExecutionResult>)sendGenericMethodInfo.Invoke(mediator, new[] { commandRequest, CancellationToken.None });

            return result;
        }

        /// <summary>
        /// Resolves service from container.
        /// </summary>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <returns>Service instance.</returns>
        public TService GetService<TService>()
        {
            return this.serviceProvider.GetRequiredService<TService>();
        }

        /// <summary>
        /// Creates test flow.
        /// </summary>
        /// <returns>Created flow.</returns>
        public SubdomainTestFlow CreateFlow()
        {
            return new SubdomainTestFlow(this);
        }

        /// <summary>
        /// Creates state based test flow.
        /// </summary>
        /// <typeparam name="TState">Test flow state.</typeparam>
        /// <returns>Created flow.</returns>
        public SubdomainTestFlow<TState> CreateFlow<TState>()
            where TState : class, new()
        {
            return new SubdomainTestFlow<TState>(this);
        }
    }
}