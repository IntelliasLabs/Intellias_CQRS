using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Tests.Tools
{
    /// <summary>
    /// Handles event for all.
    /// </summary>
    public class HandlerManager
    {
        private readonly HandlerDependencyResolver resolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerManager"/> class.
        /// </summary>
        /// <param name="resolver">Resolver.</param>
        public HandlerManager(HandlerDependencyResolver resolver)
        {
            this.resolver = resolver;
        }

        /// <summary>
        /// HandleEventAsync.
        /// </summary>
        /// <typeparam name="T">Event Type.</typeparam>
        /// <param name="event">Event.</param>
        /// <returns>Simple Task.</returns>
        public Task HandleEventAsync<T>(T @event)
            where T : IEvent =>
            Task.WhenAll(resolver.ResolveEvent(@event).Select(handler => handler.HandleAsync(@event)));

        /// <summary>
        /// HandleCommandAsync.
        /// </summary>
        /// <typeparam name="T">Command Type.</typeparam>
        /// <param name="command">Command.</param>
        /// <returns>Execution Result.</returns>
        public Task<IExecutionResult> HandleCommandAsync<T>(T command)
            where T : ICommand =>
            resolver.ResolveCommand(command).HandleAsync(command);
    }
}