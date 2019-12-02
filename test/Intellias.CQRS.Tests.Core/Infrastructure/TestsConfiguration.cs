using System.IO;
using Intellias.CQRS.Tests.Core.Infrastructure.Options;
using Microsoft.Extensions.Configuration;

namespace Intellias.CQRS.Tests.Core.Infrastructure
{
    /// <summary>
    /// Tests Configuration.
    /// </summary>
    public class TestsConfiguration
    {
        private static readonly IConfiguration Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddUserSecrets(typeof(TestsConfiguration).Assembly)
            .AddEnvironmentVariables()
            .Build();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestsConfiguration"/> class.
        /// </summary>
        public TestsConfiguration()
        {
            StorageAccount = Configuration
                .GetSection(StorageAccountOptions.SectionName)
                .Get<StorageAccountOptions>();

            AzureDevOpsAccessToken = Configuration.GetValue<string>(nameof(AzureDevOpsAccessToken));
        }

        /// <summary>
        /// Storage Account.
        /// </summary>
        public StorageAccountOptions StorageAccount { get; }

        /// <summary>
        /// Azure DevOps Access Token.
        /// </summary>
        public string AzureDevOpsAccessToken { get; }
    }
}