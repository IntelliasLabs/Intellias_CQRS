namespace Intellias.CQRS.Configuration.Environments
{
    /// <summary>
    /// Ingrowth host environment.
    /// </summary>
    public interface IIngrowthHostEnvironment
    {
        /// <summary>
        /// Is environment.
        /// </summary>
        /// <param name="environmentName">Environment name.</param>
        /// <returns>True if environment equal current environment.</returns>
        bool IsEnvironment(string environmentName);

        /// <summary>
        /// Is local environment.
        /// </summary>
        /// <returns>True if environment local.</returns>
        bool IsLocal();

        /// <summary>
        /// Is development environment.
        /// </summary>
        /// <returns>True if environment development.</returns>
        bool IsDevelopment();

        /// <summary>
        /// Is staging environment.
        /// </summary>
        /// <returns>True if environment staging.</returns>
        bool IsStaging();

        /// <summary>
        /// Is production environment.
        /// </summary>
        /// <returns>True if environment production.</returns>
        bool IsProduction();
    }
}
