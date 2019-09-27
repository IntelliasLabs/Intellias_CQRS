using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Signals;
using MediatR;

namespace Intellias.CQRS.Pipelines.CommandHandlers.Behaviors
{
    /// <summary>
    /// Publishes integration events and signals to the outer world.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to be handled.</typeparam>
    public class PublisherBehavior<TCommand> : IPipelineBehavior<CommandRequest<TCommand>, IExecutionResult>
        where TCommand : Command
    {
        private readonly IEventBus eventBus;
        private readonly IReportBus reportBus;
        private readonly IIntegrationEventStore integrationEventStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherBehavior{TCommand}"/> class.
        /// </summary>
        /// <param name="eventBus">Events bus.</param>
        /// <param name="reportBus">Signals bus.</param>
        /// <param name="integrationEventStore">Store of integration events.</param>
        public PublisherBehavior(IEventBus eventBus, IReportBus reportBus, IIntegrationEventStore integrationEventStore)
        {
            this.eventBus = eventBus;
            this.reportBus = reportBus;
            this.integrationEventStore = integrationEventStore;
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> Handle(
            CommandRequest<TCommand> request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<IExecutionResult> next)
        {
            var result = await next();
            if (!result.Success)
            {
                await reportBus.PublishAsync(new OperationFailedSignal(request.Command, (FailedResult)result));
                return result;
            }

            if (result is IntegrationEventExecutionResult e)
            {
                // Publish domain event.
                await eventBus.PublishAsync(e.Event);

                // Mark domain event as published.
                await integrationEventStore.MarkAsPublishedAsync(e.Event);
            }

            await reportBus.PublishAsync(new OperationCompletedSignal(request.Command));

            return result;
        }
    }
}