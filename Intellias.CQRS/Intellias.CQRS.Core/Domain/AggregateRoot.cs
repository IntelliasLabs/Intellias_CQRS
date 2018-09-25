using Product.Domain.Core.Domain.Exceptions;
using Product.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Product.Domain.Core.Domain
{
    public abstract class AggregateRoot : Entity, IAggregateRoot
    {
        private readonly List<Event> _changes = new List<Event>();

        public int Version { get; protected set; }

        public Event[] GetUncommittedChanges()
        {
            lock (_changes)
            {
                return _changes.ToArray();
            }
        }

        public Event[] FlushUncommitedChanges()
        {
            lock (_changes)
            {
                var changes = _changes.ToArray();
                var i = 0;
                foreach (var @event in changes)
                {
                    if (@event.Id == string.Empty && Id == string.Empty)
                    {
                        throw new AggregateOrEventMissingIdException(GetType(), @event.GetType());
                    }
                    if (@event.Id == string.Empty)
                    {
                        @event.Id = Id;
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

        public void LoadFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history)
            {
                if (e.Version != Version + 1)
                {
                    throw new EventsOutOfOrderException(e.Id);
                }
                ApplyChange(e, false);
            }
        }

        protected void ApplyChange(Event @event)
        {
            ApplyChange(@event, true);
        }

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
                    Id = @event.Id;
                    Version++;
                }
            }
        }

        protected virtual void Apply(Event @event)
        {
            Apply(@event);
        }
    }
}
