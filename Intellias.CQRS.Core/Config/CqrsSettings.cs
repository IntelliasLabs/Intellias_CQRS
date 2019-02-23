using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Intellias.CQRS.Core.Config
{
    /// <summary>
    /// Extensions methods for CQRS config
    /// </summary>
    public static class CqrsSettings
    {
        /// <summary>
        /// Configures JSON serializer globally
        /// Settings will automatically be used by JsonConvert.SerializeObject/DeserializeObject
        /// </summary>
        public static Func<JsonSerializerSettings> JsonConfig => 
            () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
    }
}
