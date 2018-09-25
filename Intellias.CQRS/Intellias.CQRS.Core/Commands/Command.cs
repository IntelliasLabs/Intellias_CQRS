using System;

namespace Product.Domain.Core.Commands
{
    public abstract class Command : ICommand
    {
        public int ExpectedVersion { get; set; }

        public virtual string Id { get; set; }
    }
}
