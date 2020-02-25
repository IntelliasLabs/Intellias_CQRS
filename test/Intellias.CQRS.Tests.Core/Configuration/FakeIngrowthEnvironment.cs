using Intellias.CQRS.Configuration.Environments;

namespace Intellias.CQRS.Tests.Core.Configuration
{
    /// <summary>
    /// Fake ingrowth environment.
    /// </summary>
    public class FakeIngrowthEnvironment : IIngrowthEnvironment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeIngrowthEnvironment"/> class.
        /// </summary>
        /// <param name="environmentName">Environment name.</param>
        public FakeIngrowthEnvironment(string environmentName)
        {
            Name = environmentName;
        }

        /// <inheritdoc />
        public string Name { get; private set; }

        /// <inheritdoc />
        public bool IsEnvironment(string environmentName) => Name == environmentName;

        /// <inheritdoc />
        public bool IsLocal() => IsEnvironment(IngrowthEnvironment.Local);

        /// <inheritdoc/>
        public bool IsDevelopment() => IsEnvironment(IngrowthEnvironment.Development);

        /// <inheritdoc/>
        public bool IsStaging() => IsEnvironment(IngrowthEnvironment.Staging);

        /// <inheritdoc/>
        public bool IsProduction() => IsEnvironment(IngrowthEnvironment.Production);

        /// <summary>
        /// Set environment name.
        /// </summary>
        /// <param name="environmentName">Environment name.</param>
        public void SetEnvironmentName(string environmentName) => Name = environmentName;
    }
}
