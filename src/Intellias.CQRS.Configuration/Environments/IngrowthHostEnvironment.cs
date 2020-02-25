using System;

namespace Intellias.CQRS.Configuration.Environments
{
    /// <summary>
    /// Ingrowth host environment.
    /// </summary>
    public class IngrowthHostEnvironment : IIngrowthHostEnvironment
    {
        /// <summary>
        /// Local.
        /// </summary>
        public const string Local = "local";

        /// <summary>
        /// Development.
        /// </summary>
        public const string Development = "dev";

        /// <summary>
        /// Testing.
        /// </summary>
        public const string Testing = "test";

        /// <summary>
        /// Staging.
        /// </summary>
        public const string Staging = "stage";

        /// <summary>
        /// Production.
        /// </summary>
        public const string Production = "prod";

        private readonly Lazy<string> currentEnvironmentName
            = new Lazy<string>(() => Environment.GetEnvironmentVariable("INGROWTH_ENVIRONMENT"));

        /// <summary>
        /// Current environment name.
        /// </summary>
        public string EnvironmentName => currentEnvironmentName.Value;

        /// <inheritdoc/>
        public bool IsEnvironment(string environmentName)
            => currentEnvironmentName.Value == environmentName;

        /// <inheritdoc />
        public bool IsLocal() => IsEnvironment(Local);

        /// <inheritdoc/>
        public bool IsDevelopment() => IsEnvironment(Development);

        /// <inheritdoc/>
        public bool IsStaging() => IsEnvironment(Staging);

        /// <inheritdoc/>
        public bool IsProduction() => IsEnvironment(Production);
    }
}
