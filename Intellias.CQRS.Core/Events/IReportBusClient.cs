using System;
using System.Threading.Tasks;

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
        void Subscribe(Func<IEvent, Task> handler);
    }
}
