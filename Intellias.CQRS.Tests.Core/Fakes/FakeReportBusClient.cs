using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <summary>
    /// Report bus for testing purposes
    /// </summary>
    public class FakeReportBusClient : IReportBusClient
    {
        private readonly List<Func<IEvent, Task>> handlers = new List<Func<IEvent, Task>>();

        /// <inheritdoc />
        public void Subscribe(Func<IEvent, Task> handler)
        {
            handlers.Add(handler);
        }

        /// <summary>
        /// Pubh test event to bus
        /// </summary>
        /// <param name="e">event</param>
        /// <returns></returns>
        public async Task PushTestEventAsync(IEvent e)
        {
            foreach (var handler in handlers)
            {
                await handler(e);
            }
        }
    }
}
