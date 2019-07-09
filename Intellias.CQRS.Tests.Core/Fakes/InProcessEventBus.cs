using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <inheritdoc />
    public class InProcessEventBus : IEventBus
    {
        private readonly Dictionary<Type, List<IEventHandler<IEvent>>> funcs;

        /// <summary>
        /// Creates message bus
        /// </summary>
        public InProcessEventBus()
        {
            funcs = new Dictionary<Type, List<IEventHandler<IEvent>>>();
        }

        /// <summary>
        /// Add event handler
        /// </summary>
        /// <param name="handler">event handler</param>
        /// <typeparam name="T"></typeparam>
        public void AddHandler<T>(IEventHandler<T> handler) where T : IEvent
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var abstractHandler = (IEventHandler<IEvent>)new EventHandlerWrapper<T>(handler);

            if (funcs.ContainsKey(typeof(T)))
            {
                funcs[typeof(T)].Add(abstractHandler);
            }
            else
            {
                var list = new List<IEventHandler<IEvent>> { abstractHandler };
                funcs.Add(typeof(T), list);
            }
        }

        /// <summary>
        /// Register all implementations of Event handlers
        /// </summary>
        /// <param name="eventHandler">event handler</param>
        public void AddAllHandlers(object eventHandler)
        {
            // Here we verify that we take all interfaces IEventHandler<IEvent>
            var interfaces = eventHandler.GetType().GetInterfaces()
                .Where(i => i.GetGenericTypeDefinition() == typeof(IEventHandler<>));

            foreach (var t in interfaces)
            {
                var eventType = t.GenericTypeArguments.Single();

                var constructedWrapper = typeof(EventHandlerWrapper<>).MakeGenericType(eventType);
                var abstractHandler = (IEventHandler<IEvent>)Activator.CreateInstance(constructedWrapper, eventHandler);

                if (funcs.ContainsKey(eventType))
                {
                    funcs[eventType].Add(abstractHandler);
                }
                else
                {
                    var list = new List<IEventHandler<IEvent>> { abstractHandler };
                    funcs.Add(eventType, list);
                }
            }
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> PublishAsync(IEvent msg)
        {
            if (funcs.TryGetValue(msg.GetType(), out var funcsList))
            {
                foreach (var func in funcsList)
                {
                    await func.HandleAsync(msg);
                }
            }

            return await Task.FromResult(ExecutionResult.Successful);
        }
    }
}
