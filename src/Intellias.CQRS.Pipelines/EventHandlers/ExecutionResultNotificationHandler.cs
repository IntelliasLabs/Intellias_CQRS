using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Intellias.CQRS.Pipelines.EventHandlers
{
    /// <summary>
    /// Execution result notification handler.
    /// </summary>
    public class ExecutionResultNotificationHandler :
        INotificationHandler<QueryModelChangedNotification>,
        INotificationHandler<EventAlreadyAppliedNotification>
    {
        private readonly IReportBus reportBus;
        private readonly ILogger<ExecutionResultNotificationHandler> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionResultNotificationHandler"/> class.
        /// </summary>
        /// <param name="reportBus">Report bus.</param>
        /// <param name="logger">Logger.</param>
        public ExecutionResultNotificationHandler(
            IReportBus reportBus,
            ILogger<ExecutionResultNotificationHandler> logger)
        {
            this.reportBus = reportBus;
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task Handle(QueryModelChangedNotification notification, CancellationToken cancellationToken)
        {
            if (notification.IsPrivate)
            {
                logger.LogDebug(
                    "Query model '{QueryModelType}' of id '{QueryModelId}' signal is private and wouldn't be published.",
                    notification.Signal.QueryModelType,
                    notification.Signal.QueryModelId);

                return;
            }

            await reportBus.PublishAsync(notification.Signal);
        }

        /// <inheritdoc />
        public Task Handle(EventAlreadyAppliedNotification notification, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Event '{notification.Event.Id}' already applied to query model '{notification.QueryModelType}'.");

            return Task.CompletedTask;
        }
    }
}