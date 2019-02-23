using Intellias.CQRS.Core.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Intellias.CQRS.AzureTable.Core
{
    /// <summary>
    /// Configures Azure Tables
    /// </summary>
    public static class AzureTableConfigurator
    {
        /// <summary>
        /// Creates Azure Cloud Table with specific name
        /// </summary>
        /// <param name="cloudTableName">table name</param>
        /// <returns>CloudTable</returns>
        public static CloudTable CreateIfNotExist(string cloudTableName)
        {
            JsonConvert.DefaultSettings = CqrsSettings.JsonConfig;

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            var storeConnectionString = configuration.GetConnectionString("TableStorageConnection");

            var tableClient = CloudStorageAccount
                .Parse(storeConnectionString)
                .CreateCloudTableClient();

            var table = tableClient.GetTableReference(cloudTableName);
            table.CreateIfNotExistsAsync().Wait();

            return table;
        }
    }
}
