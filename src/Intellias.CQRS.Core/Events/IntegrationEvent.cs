namespace Intellias.CQRS.Core.Events
{
    /// <inheritdoc cref="IIntegrationEvent" />
    public class IntegrationEvent : Event, IIntegrationEvent
    {
        /// <inheritdoc />
        public bool IsReplay { get; set; }
    }
}