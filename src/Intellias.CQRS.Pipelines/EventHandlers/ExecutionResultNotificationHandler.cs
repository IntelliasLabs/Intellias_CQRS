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
        INotificationHandler<QueryModelUpdatedNotification>,
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

        /// <summary>
        /// Handle <see cref="QueryModelUpdatedNotification"/>.
        /// </summary>
        /// <param name="notification">Notification.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>Task.</returns>
        public async Task Handle(QueryModelUpdatedNotification notification, CancellationToken cancellationToken)
        {
            if (notification.IsReplay || notification.IsPrivate)
            {
                logger.LogInformation($"Notification of signal '{notification.Signal.GetType()}' is replay or private and wouldn't be published.");
                return;
            }

            await reportBus.PublishAsync(notification.Signal);
        }

        /// <summary>
        /// Handle <see cref="EventAlreadyAppliedNotification"/>.
        /// </summary>
        /// <param name="notification">Notification.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>Task.</returns>
        public Task Handle(EventAlreadyAppliedNotification notification, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Event '{notification.Event.Id}' already applied to query model '{notification.QueryModelType}'.");

            return Task.CompletedTask;
        }
    }
}