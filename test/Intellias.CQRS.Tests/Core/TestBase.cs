using System.Reflection;
using Intellias.CQRS.Core.Storage;
using Intellias.CQRS.Core.Tools;
using Intellias.CQRS.Tests.Core.EventHandlers;
using Intellias.CQRS.Tests.Core.Queries;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Intellias.CQRS.Tests.Core
{
    /// <summary>
    /// TestBase.
    /// </summary>
    public class TestBase
    {
        protected TestBase()
        {
            // setup our DI
            var serviceCollection = new ServiceCollection();

            IocRegistrations(serviceCollection);

            ServiceProvider = serviceCollection
                .BuildServiceProvider();
        }

        protected ServiceProvider ServiceProvider { get; }

        private static void IocRegistrations(ServiceCollection services)
        {
            services.AddHandlerManager(Assembly.GetExecutingAssembly());
            services.AddTransient<DemoEventHandlers>();
            services.AddTransient<WrappedEventHandler>();

            var queryReaderMock = Mock.Of<IQueryModelReader<TestQueryModel>>();
            services.AddTransient(_ => queryReaderMock);

            var queryWriterMock = Mock.Of<IQueryModelWriter<TestQueryModel>>();
            services.AddTransient(_ => queryWriterMock);
        }
    }
}
