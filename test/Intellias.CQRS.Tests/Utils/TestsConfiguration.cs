using System.IO;
using Intellias.CQRS.Tests.Utils.Options;
using Microsoft.Extensions.Configuration;

namespace Intellias.CQRS.Tests.Utils
{
    public class TestsConfiguration
    {
        private static readonly IConfiguration Configuration;

        static TestsConfiguration()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddUserSecrets(typeof(TestsConfiguration).Assembly)
                .AddEnvironmentVariables()
                .Build();
        }

        public TestsConfiguration()
        {
            this.StorageAccount = Configuration
                .GetSection(StorageAccountOptions.SectionName)
                .Get<StorageAccountOptions>();

            this.AzureDevOpsAccessToken = Configuration.GetValue<string>(nameof(AzureDevOpsAccessToken));
        }

        public StorageAccountOptions StorageAccount { get; }

        public string AzureDevOpsAccessToken { get; }
    }
}