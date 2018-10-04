using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Core.Tests.Fakes
{
    internal class InProcessEventStore : IEventStore
    {
        private readonly IEventBus _publisher;
        private readonly Dictionary<string, List<IEvent>> _inMemoryDb = new Dictionary<string, List<IEvent>>();

        public InProcessEventStore(IEventBus bus)
        {
            _publisher = bus;
        }

        public async Task SaveAsync(IAggregateRoot entity)
        {
            foreach (var @event in entity.Events)
            {
                _inMemoryDb.TryGetValue(@event.Id, out var list);
                if (list == null)
                {
                    list = new List<IEvent>();
                    _inMemoryDb.Add(@event.Id, list);
                }
                list.Add(@event);
                await _publisher.PublishAsync(@event);
            }
        }

        public Task<IEnumerable<IEvent>> GetAsync(string aggregateId, int fromVersion)
        {
            _inMemoryDb.TryGetValue(aggregateId, out var events);
            return Task.FromResult(events?.Where(x => x.Version > fromVersion) ?? new List<IEvent>());
        }
    }
}
