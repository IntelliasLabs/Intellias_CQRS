
using System;
using Intellias.CQRS.Core.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intellias.CQRS.Core.Config
{
    /// <summary>
    /// Deserialize IMessage to specific type that provided in TypeName property
    /// </summary>
    internal class MessageWithTypeNameJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsInterface && objectType.IsAssignableFrom(typeof(IMessage));
        }

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var typeNamePath = GetMessageTypeNamePath();
            var typeName = jObject.SelectToken(typeNamePath).ToString();
            return jObject.ToObject(Type.GetType(typeName));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Convert first letter to lower
        /// </summary>
        /// <returns></returns>
        private static string GetMessageTypeNamePath()
        {
            const string name = nameof(IMessage.TypeName);
            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }
    }
}
