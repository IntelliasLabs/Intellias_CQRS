using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Core.Tools
{
    /// <summary>
    /// Handles event for all
    /// </summary>
    public class HandlerManager
    {
        private readonly HandlerDependencyResolver resolver;

        /// <summary>
        /// Handler Manager
        /// </summary>
        /// <param name="resolver"></param>
        public HandlerManager(HandlerDependencyResolver resolver)
        {
            this.resolver = resolver;
        }


        /// <summary>
        /// HandleEventAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <returns></returns>
        public Task HandleEventAsync<T>(T @event) where T : IEvent =>
            Task.WhenAll(resolver.ResolveEvent(@event).Select(handler => handler.HandleAsync(@event)));

        /// <summary>
        /// HandleCommandAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task<IExecutionResult> HandleCommandAsync<T>(T command) where T : ICommand =>
            resolver.ResolveCommand(command).HandleAsync(command);
    }
}