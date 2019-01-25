using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Core.Tools
{
    /// <summary>
    /// Handles event for all 
    /// </summary>
    public class HandlerManager
        
    {
        private HandlerDependencyResolver Resolver { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolver"></param>
        public HandlerManager(HandlerDependencyResolver resolver)
        {
            Resolver = resolver;
        }


        /// <summary>
        /// HandleEventAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <returns></returns>
        public Task HandleEventAsync<T>(T @event) where T : IEvent =>
            Task.WhenAll(Resolver.ResolveEvent(@event).Select(handler => handler.HandleAsync(@event)));

        /// <summary>
        /// HandleCommandAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task HandleCommandAsync<T>(T command) where T : ICommand =>
            Task.WhenAll(Resolver.ResolveCommand(command).Select(handler => handler.HandleAsync(command)));
    }
}
