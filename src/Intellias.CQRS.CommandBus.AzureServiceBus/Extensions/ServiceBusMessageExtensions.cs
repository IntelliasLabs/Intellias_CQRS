using System.Text;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;
using Microsoft.Azure.ServiceBus;

namespace Intellias.CQRS.CommandBus.AzureServiceBus.Extensions
{
    internal static class ServiceBusMessageExtensions
    {
        public static Message ToBusMessage(this ICommand command) =>
            new Message(Encoding.UTF8.GetBytes(command.ToJson()))
            {
                MessageId = command.Id,
                ContentType = command.GetType().AssemblyQualifiedName,
                PartitionKey = AbstractMessage.GlobalSessionId,
                CorrelationId = command.CorrelationId,
                SessionId = AbstractMessage.GlobalSessionId,
                Label = command.GetType().Name
            };
    }
}
