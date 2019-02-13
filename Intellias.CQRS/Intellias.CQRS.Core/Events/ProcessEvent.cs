using Intellias.CQRS.Core.Processes;

namespace Intellias.CQRS.Core.Events
{
    /// <summary>
    /// Event used to pass process manager's state with event
    /// </summary>
    public abstract class ProcessEvent : Event
    {
        /// <summary>
        /// Process-manager's state
        /// </summary>
        public IProcess Process { get; set; }
    }
}
