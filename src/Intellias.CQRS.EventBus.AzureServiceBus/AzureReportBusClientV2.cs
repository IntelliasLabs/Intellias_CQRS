using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace Intellias.CQRS.EventBus.AzureServiceBus
{
    /// <inheritdoc />
    [Obsolete("Please use see AzureReportBusClient as a copy of this class.")]
    [ExcludeFromCodeCoverage]
    public class AzureReportBusClientV2 : IReportBusClient
    {
        private readonly ISubscriptionClient sub;
        private readonly ILogger<AzureReportBusClientV2> log;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureReportBusClientV2"/> class.
        /// </summary>
        /// <param name="log">ILogger.</param>
        /// <param name="sub">Subscription Type.</param>
        public AzureReportBusClientV2(ILogger<AzureReportBusClientV2> log, ISubscriptionClient sub)
        {
            this.sub = sub;
            this.log = log;
        }

        /// <inheritdoc />
        public void Subscribe(Func<IMessage, Task> handler)
        {
            var options = new SessionHandlerOptions(ExceptionReceivedHandlerAsync)
            {
                AutoComplete = false
            };

            // Register the function that processes messages.
            sub.RegisterSessionHandler(
                async (session, msg, token) =>
                {
                    var json = Encoding.UTF8.GetString(msg.Body);
                    var message = json.FromJson<IMessage>();

                    // Invoke handler
                    if (handler != null)
                    {
                        await handler(message);
                    }

                    await session.CompleteAsync(msg.SystemProperties.LockToken);
                }, options);
        }

        private Task ExceptionReceivedHandlerAsync(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            log.LogError(exceptionReceivedEventArgs.Exception, exceptionReceivedEventArgs.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
