using Newtonsoft.Json;

namespace Intellias.CQRS.Persistence.AzureStorage.Common
{
    /// <summary>
    /// Default <see cref="JsonSerializerSettings"/> for Azure Table Storage.
    /// </summary>
    public static class TableStorageJsonSerializerSettings
    {
        /// <summary>
        /// Returns preconfigured <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <returns>Preconfigured settings.</returns>
        public static JsonSerializerSettings GetDefault()
        {
            return new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };
        }
    }
}