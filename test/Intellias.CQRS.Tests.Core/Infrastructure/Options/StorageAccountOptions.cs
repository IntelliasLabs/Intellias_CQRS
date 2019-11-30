namespace Intellias.CQRS.Tests.Core.Infrastructure.Options
{
    /// <summary>
    /// Storage Account Options.
    /// </summary>
    public class StorageAccountOptions
    {
        /// <summary>
        /// Section Name.
        /// </summary>
        public const string SectionName = "StorageAccount";

        /// <summary>
        /// Connection String.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;
    }
}