using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;

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

        /// <inheritdoc />
        public async Task<IExecutionResult> PublishAsync(IEvent msg)
        {
            var funcsList = funcs[msg.GetType()];

            foreach (var func in funcsList)
            {
                await func.HandleAsync(msg);
            }

            return await Task.FromResult(ExecutionResult.Success);
        }
    }
}
