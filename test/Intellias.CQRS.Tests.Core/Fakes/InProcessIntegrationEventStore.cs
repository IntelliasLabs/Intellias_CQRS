using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    public class InProcessIntegrationEventStore : IIntegrationEventStore
    {
        private readonly Dictionary<long, IntegrationEventEntry> store = new Dictionary<long, IntegrationEventEntry>();

        public Task SaveUnpublishedAsync(IIntegrationEvent @event)
        {
            store[@event.Created.Ticks] = new IntegrationEventEntry(@event);
            return Task.CompletedTask;
        }

        public Task MarkAsPublishedAsync(IIntegrationEvent @event)
        {
            store[@event.Created.Ticks].IsPublished = true;
            return Task.CompletedTask;
        }

        private class IntegrationEventEntry
        {
            public IntegrationEventEntry(IIntegrationEvent integrationEvent)
            {
                IntegrationEvent = integrationEvent;
            }

            public IIntegrationEvent IntegrationEvent { get; }

            public bool IsPublished { get; set; }
        }
    }
}