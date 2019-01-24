using System;
using System.Collections.Generic;
using System.Linq;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Core.Tools
{
    /// <summary>
    /// Used to return all handler instanses from handler's assembly 
    /// </summary>
    public class EventHandlerDependencyResolver
    {
        private IServiceProvider Service { get; }
        private EventHandlerAssemblyResolver AssemblyResolver { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="service"></param>
        /// <param name="assemblyResolver"></param>
        public EventHandlerDependencyResolver(
            IServiceProvider service,
            EventHandlerAssemblyResolver assemblyResolver)
        {
            Service = service;
            AssemblyResolver = assemblyResolver;
        }


        /// <summary>
        /// Resolves all event handlers
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public IEnumerable<IEventHandler<T>> Resolve<T>(T @event)
            where T : IEvent
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            var handlerType = typeof(IEventHandler<T>);

            return AssemblyResolver
                .Assembly
                .GetTypes()
                .Where(t => handlerType.IsAssignableFrom(t))
                .Select(type =>
                    {
                        var service = (IEventHandler<T>)Service.GetService(type);

                        if (service == null)
                        {
                            throw new ArgumentNullException($"'{nameof(service)}'. Type '{type.Name}' cannot be resolved by service provider.");
                        }

                        return service;
                    });
        }
    }
}
