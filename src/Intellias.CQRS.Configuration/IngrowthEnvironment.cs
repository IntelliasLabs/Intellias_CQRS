using System;
using System.Diagnostics.CodeAnalysis;

namespace Intellias.CQRS.Configuration
{
    /// <summary>
    /// Ingrowth environment.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class IngrowthEnvironment
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

        private static Lazy<string> currentEnvironmentName
            = new Lazy<string>(() => Environment.GetEnvironmentVariable("INGROWTH_ENVIRONMENT"));

        /// <summary>
        /// Current environment name.
        /// </summary>
        public static string EnvironmentName => currentEnvironmentName.Value;

        /// <summary>
        /// Is environment.
        /// </summary>
        /// <param name="environmentName">Environment name.</param>
        /// <returns>True if environment equal current environment.</returns>
        public static bool IsEnvironment(string environmentName)
            => currentEnvironmentName.Value == environmentName;

        /// <summary>
        /// Is local environment.
        /// </summary>
        /// <returns>True if environment local.</returns>
        public static bool IsLocal() => IsEnvironment(Local);

        /// <summary>
        /// Is staging environment.
        /// </summary>
        /// <returns>True if environment staging.</returns>
        public static bool IsStaging() => IsEnvironment(Staging);

        /// <summary>
        /// Is production environment.
        /// </summary>
        /// <returns>True if environment production.</returns>
        public static bool IsProduction() => IsEnvironment(Production);
    }
}
