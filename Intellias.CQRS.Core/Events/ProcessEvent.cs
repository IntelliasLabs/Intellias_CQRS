using Intellias.CQRS.Core.Processes;

namespace Intellias.CQRS.Core.Events
{
    /// <summary>
    /// Event used to pass process manager's state with event
    /// </summary>
    public abstract class ProcessEvent : Event
    {
        /// <summary>
        /// Process Event
        /// </summary>
        /// <param name="process"></param>
        protected ProcessEvent(IProcess process)
        {
            Process = process;
        }

        /// <summary>
        /// Process-manager's state
        /// </summary>
        public IProcess Process { get; set; }
    }
}
