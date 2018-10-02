using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Intellias.CQRS.Core.Config
{
    /// <summary>
    /// Extensions methods for CQRS config
    /// </summary>
    public static class CqrsConfigExtensions
    {
        /// <summary>
        /// Configures JSON serializer globally
        /// Settings will automatically be used by JsonConvert.SerializeObject/DeserializeObject
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static void ConfigureJson(this Func<JsonSerializerSettings> config)
        {
            config = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }
    }
}
