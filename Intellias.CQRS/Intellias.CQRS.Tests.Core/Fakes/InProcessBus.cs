using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <inheritdoc />
    public class InProcessBus : IMessageBus<IMessage, IExecutionResult>
    {
        private readonly Dictionary<Type, List<IHandler<IMessage, IExecutionResult>>> funcs;

        /// <summary>
        /// Creates message bus
        /// </summary>
        public InProcessBus()
        {
            funcs = new Dictionary<Type, List<IHandler<IMessage, IExecutionResult>>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        public void AddHandler<T, TR>(IHandler<T, TR> handler) where T : IMessage where TR : IExecutionResult
        {
            var abstractHandler = new HandlerWrapper<T, TR>(handler);

            if (funcs.ContainsKey(typeof(T)))
            {
                funcs[typeof(T)].Add(abstractHandler);
            }
            else
            {
                var list = new List<IHandler<IMessage, IExecutionResult>> { abstractHandler };
                funcs.Add(typeof(T), list);
            }
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> PublishAsync(IMessage msg)
        {
            var results = new List<IExecutionResult>();

            var funcsList = funcs[msg.GetType()];

            foreach(var func in funcsList)
            {
                var result = await func.HandleAsync(msg);
                results.Add(result);
            }
            
            // Command result
            if (results.Count == 1)
            {
                return await Task.FromResult(results.Single());
            }

            return await Task.FromResult(CommandResult.Success);
        }
    }
}
