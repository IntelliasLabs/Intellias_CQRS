using System;
using System.Collections.Generic;
using Intellias.CQRS.Core.Domain.Exceptions;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Core.Domain
{
    /// <inheritdoc cref="IAggregateRoot" />
    public abstract class AggregateRoot : Entity, IAggregateRoot
    {
        private readonly List<Event> _changes = new List<Event>();

        /// <inheritdoc />
        public int Version { get; protected set; }

        /// <summary>
        /// Get uncommited changes to AR store
        /// </summary>
        /// <returns></returns>
        public Event[] GetUncommittedChanges()
        {
            lock (_changes)
            {
                return _changes.ToArray();
            }
        }

        /// <summary>
        /// Flush changes
        /// </summary>
        /// <returns></returns>
        public Event[] FlushUncommitedChanges()
        {
            lock (_changes)
            {
                var changes = _changes.ToArray();
                var i = 0;
                foreach (var @event in changes)
                {
                    if (string.IsNullOrWhiteSpace(@event.AggregateRootId) && string.IsNullOrWhiteSpace(Id))
                    {
                        throw new AggregateOrEventMissingIdException(GetType(), @event.GetType());
                    }
                    if (string.IsNullOrWhiteSpace(@event.AggregateRootId))
                    {
                        @event.AggregateRootId = Id;
                    }
                    i++;
                    @event.Version = Version + i;
                    @event.Created = DateTime.UtcNow;
                }
                Version = Version + _changes.Count;
                _changes.Clear();
                return changes;
            }
        }

        /// <summary>
        /// Load event history
        /// </summary>
        /// <param name="history"></param>
        public void LoadFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history)
            {
                if (e.Version != Version + 1)
                {
                    throw new EventsOutOfOrderException(e.AggregateRootId);
                }
                ApplyChange(e, false);
            }
        }

        /// <summary>
        /// Apply an event
        /// </summary>
        /// <param name="event">Event</param>
        protected void ApplyChange(Event @event)
        {
            ApplyChange(@event, true);
        }

        /// <summary>
        /// Apply new event
        /// </summary>
        /// <param name="event">Event</param>
        /// <param name="isNew">is new?</param>
        private void ApplyChange(Event @event, bool isNew)
        {
            lock (_changes)
            {
                Apply(@event);
                if (isNew)
                {
                    _changes.Add(@event);
                }
                else
                {
                    Id = @event.AggregateRootId;
                    Version++;
                }
            }
        }

        /// <summary>
        /// Apply event
        /// </summary>
        /// <param name="event">Event</param>
        protected virtual void Apply(Event @event)
        {
            Apply(@event);
        }
    }
}
