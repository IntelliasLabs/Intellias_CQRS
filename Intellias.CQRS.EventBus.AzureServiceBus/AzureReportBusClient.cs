using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Microsoft.Azure.ServiceBus;

namespace Intellias.CQRS.EventBus.AzureServiceBus
{
    /// <inheritdoc />
    public class AzureReportBusClient : IReportBusClient
    {
        private readonly ISubscriptionClient sub;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="sub"></param>
        public AzureReportBusClient(ISubscriptionClient sub)
        {
            this.sub = sub;
        }

        /// <inheritdoc />
        public void Subscribe(Func<IMessage, Task> handler)
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandlerAsync)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            // Register the function that processes messages.
            sub.RegisterMessageHandler(async (msg, token) => {
                var json = Encoding.UTF8.GetString(msg.Body);
                var message = json.MessageFromJson();

                // Invoke handler
                if (handler != null)
                {
                    await handler(message);
                }

                // Complete the message so that it is not received again.
                await sub.CompleteAsync(msg.SystemProperties.LockToken);
            }, messageHandlerOptions);
        }

        static Task ExceptionReceivedHandlerAsync(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Trace.TraceError($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            return Task.CompletedTask;
        }
    }
}