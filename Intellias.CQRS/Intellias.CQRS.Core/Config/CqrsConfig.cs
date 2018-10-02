using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace Intellias.CQRS.Core.Config
{
    /// <summary>
    /// Sets the global config of CQRS packages
    /// </summary>
    public class CqrsConfig
    {
        /// <summary>
        /// Config settings
        /// </summary>
        public IConfiguration Settings { get; private set; }

        /// <summary>
        /// Initialise config for Azure Functions. 
        /// Applies config settings from local.settings.json or Azure Portal.
        /// </summary>
        /// <param name="context"></param>
        public CqrsConfig InitFunctionConfig(ExecutionContext context)
        {
            Settings = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            return this;
        }
    }
}
