using System.Reflection;
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
                .AddTransient<INotificationHandler<QueryModelUpdatedNotification>, ExecutionResultNotificationHandler>()
                .AddTransient<INotificationHandler<EventAlreadyAppliedNotification>, ExecutionResultNotificationHandler>()
                .AddTransient<IMessageDispatcher, MessageDispatcher>();

            return services;
        }
    }
}