using System;
using System.Collections.Generic;
using System.Text;

namespace Product.Domain.Core.Domain
{
    public interface IAggregateRoot : IEntity
    {
        int Version { get; }
    }
}
