using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Intellias.CQRS.Pipelines
{
    /// <summary>
    /// Mediator that handles notifications in parallel.
    /// </summary>
    public class ParallelMediator : Mediator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelMediator"/> class.
        /// </summary>
        /// <param name="serviceFactory">Service factory.</param>
        public ParallelMediator(ServiceFactory serviceFactory)
            : base(serviceFactory)
        {
        }

        /// <inheritdoc />
        protected override Task PublishCore(IEnumerable<Func<Task>> allHandlers)
        {
            return Task.WhenAll(allHandlers.Select(handler => handler()));
        }
    }
}