using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Core.Tools
{
    /// <summary>
    /// Used to return all handler instanses from handler's assembly 
    /// </summary>
    public class HandlersDependancyResolver
    {
        private IServiceProvider Service { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="service"></param>
        public HandlersDependancyResolver(
            IServiceProvider service)
        {
            Service = service;
        }


        /// <summary>
        /// Resolves all event handlers
        /// </summary>
        /// <param name="event"></param>
        /// <param name="handlersAssembly"></param>
        /// <returns></returns>
        public IEnumerable<IEventHandler<T>> Resolve<T>(T @event, Assembly handlersAssembly)
            where T : IEvent
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            var handlerType = typeof(IEventHandler<T>);

            return handlersAssembly
                .GetTypes()
                .Where(t => handlerType.IsAssignableFrom(t))
                .Select(type =>
                    (IEventHandler<T>)Service.GetService(type));
        }
    }
}
