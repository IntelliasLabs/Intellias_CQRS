using System;
using Intellias.CQRS.Core.Commands;

namespace Intellias.CQRS.Tests.Core.Pipelines.Builders
{
    public class CommandSeed<TCommand>
        where TCommand : Command
    {
        public Action<TCommand>? Setup { get; set; }

        public Guid? UserId { get; set; }

        public string? AggregateRootId { get; set; }
    }
}