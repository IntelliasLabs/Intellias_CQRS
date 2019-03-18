using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <inheritdoc />
    public class InProcessEventStore : IEventStore
    {
        private readonly IEventBus _publisher;
        private readonly Dictionary<string, List<IEvent>> _inMemoryDb = new Dictionary<string, List<IEvent>>();

        /// <summary>
        /// Cretes event store
        /// </summary>
        /// <param name="bus">event bus</param>
        public InProcessEventStore(IEventBus bus)
        {
            _publisher = bus;
        }

        /// <inheritdoc />
        public async Task SaveAsync(IAggregateRoot entity)
        {
            foreach (var @event in entity.Events)
            {
                _inMemoryDb.TryGetValue(@event.AggregateRootId, out var list);
                if (list == null)
                {
                    list = new List<IEvent>();
                    _inMemoryDb.Add(@event.AggregateRootId, list);
                }
                list.Add(@event);
                await _publisher.PublishAsync(@event);
            }
        }

        /// <inheritdoc />
        public Task<IEnumerable<IEvent>> GetAsync(string aggregateId, int fromVersion)
        {
            var arExist = _inMemoryDb.TryGetValue(aggregateId, out var events);

            return arExist
                ? Task.FromResult(events?.Where(x => x.Version > fromVersion) ?? new List<IEvent>())
                : throw new KeyNotFoundException($"Aggregate Root with id = '{aggregateId}' hasn't been found");
        }
    }
}
