using Intellias.CQRS.Core.Storage;
using Intellias.CQRS.Core.Tools;
using Intellias.CQRS.Tests.Core.Queries;
using Intellias.CQRS.Tests.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Intellias.CQRS.Tests.Core
{
    /// <summary>
    /// TestBase
    /// </summary>
    public class TestBase
    {
        /// <summary>
        /// ServiceProvider
        /// </summary>
        protected ServiceProvider ServiceProvider { get; }

        /// <summary>
        /// ctor
        /// </summary>
        protected TestBase()
        {
            //setup our DI
            var serviceCollection = new ServiceCollection();

            IocRegistrations(serviceCollection);

            ServiceProvider = serviceCollection
                .BuildServiceProvider();
        }

        private static void IocRegistrations(ServiceCollection services)
        {
            services.AddHandlerManager<DemoEventHandlers>();
            services.AddTransient<DemoEventHandlers>();

            var queryStoreMock = Mock.Of<IQueryModelStore<TestQueryModel>>();
            services.AddTransient(_ => queryStoreMock);
        }
    }
}
