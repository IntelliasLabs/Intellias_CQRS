using Product.Domain.Core.Messages;
using System.Threading.Tasks;

namespace Product.Domain.Core.Events
{
    /// <summary>
    /// Event handler abstraction
    /// </summary>
    /// <typeparam name="T">Type of event</typeparam>
    public interface IEventHandler<T> : IHandler<T, IEventResult> where T : IEvent
    {
        /// <summary>
        /// Handle message
        /// </summary>
        /// <param name="message">abstract message</param>
        /// <returns>async task awaiter</returns>
        new Task<IEventResult> Handle(T message);
    }
}
