using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Events
{
    /// <summary>
    /// Subscription client to report bus
    /// </summary>
    public interface IReportBusClient
    {
        /// <summary>
        /// Subscribe to event stream
        /// </summary>
        /// <param name="handler">Handler func</param>
        void Subscribe(Func<IMessage, Task> handler);
    }
}
