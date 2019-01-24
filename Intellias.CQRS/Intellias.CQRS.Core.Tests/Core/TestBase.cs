using Intellias.CQRS.Core.Storage;
using Intellias.CQRS.Core.Tests.EventHandlers;
using Intellias.CQRS.Core.Tools;
using Intellias.CQRS.Tests.Core.Queries;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Intellias.CQRS.Core.Tests.Core
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

        private void IocRegistrations(ServiceCollection services)
        {
            services.AddSingleton<HandlersDependancyResolver>();

            services.AddTransient<DemoEventHandlers>();

            var queryStoreMock = Mock.Of<IQueryModelStore<TestQueryModel>>();
            services.AddTransient(_ => queryStoreMock);
        }
    }
}
