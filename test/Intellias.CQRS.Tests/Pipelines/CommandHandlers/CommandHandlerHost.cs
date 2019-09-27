using System;
using System.Reflection;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.DomainServices;
using Intellias.CQRS.Pipelines;
using Intellias.CQRS.Pipelines.CommandHandlers;
using Intellias.CQRS.Pipelines.Transactions;
using Intellias.CQRS.Tests.Core.Fakes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Intellias.CQRS.Tests.Pipelines.CommandHandlers
{
    public class CommandHandlerHost
    {
        private static readonly Assembly[] AssembliesWithCommandHandlers = { typeof(CommandHandlerHost).Assembly };
        private static readonly Assembly[] AssembliesWithCommands = { typeof(CommandHandlerHost).Assembly };

        private readonly IServiceCollection serviceCollection;
        private IServiceProvider serviceProvider;

        public CommandHandlerHost()
        {
            serviceCollection = new ServiceCollection()
                .AddCommandHandlers(AssembliesWithCommandHandlers, AssembliesWithCommands)
                .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
                .AddSingleton<IEventStore, InProcessEventStore>()
                .AddSingleton<IAggregateStore, AggregateStore>()
                .AddSingleton<ITransactionStore, InProcessTransactionStore>()
                .AddSingleton<IIntegrationEventStore, InProcessIntegrationEventStore>()
                .AddSingleton<IReportBus, InProcessReportBus>()
                .AddSingleton<IEventBus, InProcessEventBus>()
                .AddSingleton<IUniqueConstraintService, InProcessUniqueConstraintService>();

            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public void RegisterHandler<THandler, TCommand>()
            where THandler : class, IRequestHandler<CommandRequest<TCommand>, IExecutionResult>
            where TCommand : Command
        {
            serviceCollection.AddSingleton<IRequestHandler<CommandRequest<TCommand>, IExecutionResult>, THandler>();
            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public async Task<IExecutionResult> SendAsync<T>(T command)
            where T : Command
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var result = await mediator.Send(new CommandRequest<T>(command));

            return result;
        }
    }
}