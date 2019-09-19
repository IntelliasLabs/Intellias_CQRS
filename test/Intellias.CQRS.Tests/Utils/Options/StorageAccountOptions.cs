namespace Intellias.CQRS.Tests.Utils.Options
{
    public class StorageAccountOptions
    {
        public const string SectionName = nameof(StorageAccount);

        public string ConnectionString { get; set; } = string.Empty;
    }
}