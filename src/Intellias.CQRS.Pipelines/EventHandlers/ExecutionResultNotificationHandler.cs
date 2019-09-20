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
        private readonly ILogger<ExecutionResultNotificationHandler> log;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionResultNotificationHandler"/> class.
        /// </summary>
        /// <param name="reportBus">Report bus.</param>
        /// <param name="log">Logger.</param>
        public ExecutionResultNotificationHandler(
            IReportBus reportBus,
            ILogger<ExecutionResultNotificationHandler> log)
        {
            this.reportBus = reportBus;
            this.log = log;
        }

        /// <summary>
        /// Handle QueryModelUpdatedNotification.
        /// </summary>
        /// <param name="notification">Notification.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>Task.</returns>
        public async Task Handle(QueryModelUpdatedNotification notification, CancellationToken cancellationToken)
        {
            if (!notification.IsReplay)
            {
                await reportBus.PublishAsync(notification.Signal);
            }
        }

        /// <summary>
        /// Handle EventAlreadyAppliedNotification.
        /// </summary>
        /// <param name="notification">Notification.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>Task.</returns>
        public Task Handle(EventAlreadyAppliedNotification notification, CancellationToken cancellationToken)
        {
            log.LogInformation($"Event '{notification.Event.Id}' already applied to query model '{notification.QueryModelType}'.");

            return Task.CompletedTask;
        }
    }
}