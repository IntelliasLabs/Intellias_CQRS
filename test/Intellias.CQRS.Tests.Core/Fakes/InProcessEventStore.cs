﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <inheritdoc />
    public class InProcessEventStore : IEventStore
    {
        private readonly Dictionary<string, List<IEvent>> inMemoryDb = new Dictionary<string, List<IEvent>>();

        /// <inheritdoc />
        public Task<IReadOnlyCollection<IEvent>> SaveAsync(IAggregateRoot entity)
        {
            foreach (var @event in entity.Events)
            {
                inMemoryDb.TryGetValue(@event.AggregateRootId, out var list);
                if (list == null)
                {
                    list = new List<IEvent>();
                    inMemoryDb.Add(@event.AggregateRootId, list);
                }

                list.Add(@event);
            }

            return Task.FromResult(entity.Events);
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<IEvent>> GetAsync(string aggregateId, int fromVersion)
        {
            var arExist = inMemoryDb.TryGetValue(aggregateId, out var events);

            return arExist
                ? Task.FromResult((IReadOnlyCollection<IEvent>)(events?.Where(x => x.Version > fromVersion) ?? new List<IEvent>()).ToList().AsReadOnly())
                : throw new KeyNotFoundException($"Aggregate Root with id = '{aggregateId}' hasn't been found");
        }
    }
}
