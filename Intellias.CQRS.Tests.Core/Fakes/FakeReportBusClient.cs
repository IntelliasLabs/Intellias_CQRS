using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <summary>
    /// Report bus for testing purposes
    /// </summary>
    public class FakeReportBusClient : IReportBusClient
    {
        private readonly List<Func<IMessage, Task>> handlers = new List<Func<IMessage, Task>>();

        /// <inheritdoc />
        public void Subscribe(Func<IMessage, Task> handler)
        {
            handlers.Add(handler);
        }

        /// <summary>
        /// Pubh test event to bus
        /// </summary>
        /// <param name="message">message</param>
        /// <returns></returns>
        public async Task PushTestEventAsync(IMessage message)
        {
            foreach (var handler in handlers)
            {
                await handler(message);
            }
        }
    }
}
