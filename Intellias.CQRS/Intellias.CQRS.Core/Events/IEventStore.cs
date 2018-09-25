using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Product.Domain.Core.Events
{
    public interface IEventStore
    {
        Task Save(IEnumerable<IEvent> events);

        Task<IEnumerable<IEvent>> Get(Guid aggregateId, int version);
    }
}
