namespace Intellias.CQRS.Core.Events
{
    /// <summary>
    /// Base interface of integration events.
    /// </summary>
    public interface IIntegrationEvent : IEvent
    {
        /// <summary>
        /// Identify that integration event triggered by the replay mechanism.
        /// </summary>
        bool IsReplay { get; set; }
    }
}