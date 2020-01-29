using System.Linq;
using System.Reflection;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Pipelines.CommandHandlers;
using Intellias.CQRS.Pipelines.CommandHandlers.Behaviors;
using Intellias.CQRS.Pipelines.EventHandlers;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Intellias.CQRS.Pipelines
{
    /// <summary>
    /// Pipeline service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers pipeline abstractions.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <param name="commandHandlersAssembly">Assembly where to look for command handlers.</param>
        /// <param name="commandsAssembly">Assembly where to look for commands.</param>
        /// <returns>Service collection with registered types.</returns>
        public static IServiceCollection AddCommandHandlers(
            this IServiceCollection services,
            Assembly commandHandlersAssembly,
            Assembly commandsAssembly)
        {
            return AddCommandHandlers(services, new[] { commandHandlersAssembly }, new[] { commandsAssembly });
        }

        /// <summary>
        /// Registers pipeline abstractions.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <param name="commandHandlersAssemblies">Assemblies where to look for command handlers.</param>
        /// <param name="commandsAssemblies">Assemblies where to look for commands.</param>
        /// <returns>Service collection with registered types.</returns>
        public static IServiceCollection AddCommandHandlers(
            this IServiceCollection services,
            Assembly[] commandHandlersAssemblies,
            Assembly[] commandsAssemblies)
        {
            services.AddMediatR(commandHandlersAssemblies)
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddTransient<IMessageDispatcher, MessageDispatcher>();

            var subdomainCommandsType = commandsAssemblies.SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces().Contains(typeof(ICommand)))
                .ToArray();

            foreach (var commandType in subdomainCommandsType)
            {
                var commandRequestType = typeof(CommandRequest<>).MakeGenericType(commandType);

                var behaviorInterfaceType = typeof(IPipelineBehavior<,>).MakeGenericType(commandRequestType, typeof(IExecutionResult));
                var publisherType = typeof(PublisherBehavior<>).MakeGenericType(commandType);
                var validatorType = typeof(ValidationBehavior<>).MakeGenericType(commandType);
                var transactionManagerType = typeof(TransactionManagerBehavior<>).MakeGenericType(commandType);

                services.AddTransient(behaviorInterfaceType, publisherType);
                services.AddTransient(behaviorInterfaceType, validatorType);
                services.AddTransient(behaviorInterfaceType, transactionManagerType);
            }

            return services;
        }

        /// <summary>
        /// Registers Integration event pipeline abstractions.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <param name="integrationEventHandlersAssemble">Assemblies where to look for integration event handlers.</param>
        /// <returns>Service collection with registered types.</returns>
        public static IServiceCollection AddEventHandlers(
            this IServiceCollection services,
            Assembly integrationEventHandlersAssemble)
        {
            services.AddMediatR(integrationEventHandlersAssemble)
                .AddTransient<IMediator, ParallelMediator>()
                .AddTransient<INotificationHandler<QueryModelUpdatedNotification>, ExecutionResultNotificationHandler>()
                .AddTransient<INotificationHandler<EventAlreadyAppliedNotification>, ExecutionResultNotificationHandler>()
                .AddTransient<IMessageDispatcher, MessageDispatcher>();

            return services;
        }
    }
}