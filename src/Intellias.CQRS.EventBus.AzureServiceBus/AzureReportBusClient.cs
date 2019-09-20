using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Microsoft.Azure.ServiceBus;

namespace Intellias.CQRS.EventBus.AzureServiceBus
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class AzureReportBusClient : IReportBusClient
    {
        private readonly ISubscriptionClient sub;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureReportBusClient"/> class.
        /// </summary>
        /// <param name="sub">Subscription Type.</param>
        public AzureReportBusClient(ISubscriptionClient sub)
        {
            this.sub = sub;
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

        private static Task ExceptionReceivedHandlerAsync(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Trace.TraceError($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            return Task.CompletedTask;
        }
    }
}