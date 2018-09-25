using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Product.Domain.Core.Events
{
    public interface IEventBus
    {
        Task Publish<T>(T @event) where T : IEvent;
    }
}
