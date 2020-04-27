using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Intellias.CQRS.Persistence.AzureStorage.Common
{
    /// <summary>
    /// Table serializer value.
    /// </summary>
    public class AzureTableValue
    {
        private const int MaxTableCellSizeBytes = 65536; // Number of bytes that fits to Azure Table cell.
        private const string DefaultPropertyNameDelimiter = "_";
        private const string NewtonsoftJsonPropertySuffix = "__Json";
        private const string GzipPropertySuffix = "__GZip";

        private static readonly DateTime AzureTableStorageMinDateTime = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new JsonConverter[] { new StringEnumConverter() },
            MissingMemberHandling = MissingMemberHandling.Ignore,
            DateParseHandling = DateParseHandling.DateTimeOffset,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

        private static readonly Dictionary<Type, Func<object, EntityProperty>> PrimitiveTypes = new Dictionary<Type, Func<object, EntityProperty>>
        {
            { typeof(string), val => new EntityProperty((string)val) },
            { typeof(byte[]), val => new EntityProperty((byte[])val) },
            { typeof(byte), val => new EntityProperty(new[] { (byte)val }) },
            { typeof(bool), val => new EntityProperty((bool)val) },
            { typeof(bool?), val => new EntityProperty((bool?)val) },
            { typeof(DateTime), val => new EntityProperty(((DateTime)val) == default ? AzureTableStorageMinDateTime : (DateTime)val) },
            { typeof(DateTime?), val => new EntityProperty(((DateTime?)val) == default(DateTime) ? AzureTableStorageMinDateTime : (DateTime?)val) },
            { typeof(DateTimeOffset), val => new EntityProperty(((DateTimeOffset)val) == default ? new DateTimeOffset(AzureTableStorageMinDateTime) : (DateTimeOffset)val) },
            { typeof(DateTimeOffset?), val => new EntityProperty(((DateTimeOffset?)val) == default(DateTimeOffset) ? new DateTimeOffset(AzureTableStorageMinDateTime) : (DateTimeOffset?)val) },
            { typeof(double), val => new EntityProperty((double)val) },
            { typeof(double?), val => new EntityProperty((double?)val) },
            { typeof(Guid), val => new EntityProperty((Guid)val) },
            { typeof(Guid?), val => new EntityProperty((Guid?)val) },
            { typeof(int), val => new EntityProperty((int)val) },
            { typeof(int?), val => new EntityProperty((int?)val) },
            { typeof(uint), val => new EntityProperty((uint)val) },
            { typeof(uint?), val => new EntityProperty((uint?)val) },
            { typeof(long), val => new EntityProperty((long)val) },
            { typeof(long?), val => new EntityProperty((long?)val) },
            { typeof(ulong), val => new EntityProperty((long)Convert.ToUInt64(val, CultureInfo.InvariantCulture)) },
            { typeof(ulong?), val => new EntityProperty((long)Convert.ToUInt64(val, CultureInfo.InvariantCulture)) },
            { typeof(TimeSpan), val => new EntityProperty(val.ToString()) },
            { typeof(TimeSpan?), val => new EntityProperty(val?.ToString()) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTableValue"/> class.
        /// </summary>
        /// <param name="path">Value for <see cref="Path"/>.</param>
        /// <param name="value">Value for <see cref="Value"/>.</param>
        public AzureTableValue(ImmutableList<string> path, object value)
            : this(path, value, AzureTableValueFormat.Raw)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTableValue"/> class.
        /// </summary>
        /// <param name="path">Value for <see cref="Path"/>.</param>
        /// <param name="value">Value for <see cref="Value"/>.</param>
        /// <param name="format">Value format.</param>
        public AzureTableValue(ImmutableList<string> path, object value, AzureTableValueFormat format)
        {
            Path = path;
            Value = value;
            Type = value?.GetType();
            Format = format;
        }

        /// <summary>
        /// Object tree path to value.
        /// </summary>
        public ImmutableList<string> Path { get; }

        /// <summary>
        /// Object value.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// <see cref="Value"/> type.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// <see cref="Value"/> type.
        /// </summary>
        public AzureTableValueFormat Format { get; }

        /// <summary>
        /// Creates <see cref="AzureTableValue"/> from table value.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="value">Value.</param>
        /// <returns>Created table value.</returns>
        public static AzureTableValue Create(string columnName, object value)
        {
            var format = AzureTableValueFormat.Raw;
            if (columnName.EndsWith(NewtonsoftJsonPropertySuffix, StringComparison.Ordinal))
            {
                columnName = columnName.Substring(0, columnName.Length - NewtonsoftJsonPropertySuffix.Length);
                format = AzureTableValueFormat.Json;
            }

            if (columnName.EndsWith(GzipPropertySuffix, StringComparison.Ordinal))
            {
                columnName = columnName.Substring(0, columnName.Length - GzipPropertySuffix.Length);
                format = AzureTableValueFormat.GZip;
            }

            var pathSegments = columnName.Split(new[] { DefaultPropertyNameDelimiter }, StringSplitOptions.RemoveEmptyEntries);
            var path = ImmutableList<string>.Empty.AddRange(pathSegments);

            return new AzureTableValue(path, value, format);
        }

        /// <summary>
        /// Serializes value to be stored in table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="entityProperty">Value.</param>
        /// <returns>True if successfully serialized.</returns>
        public bool TrySerialize(out string columnName, out EntityProperty entityProperty)
        {
            columnName = string.Join(DefaultPropertyNameDelimiter, Path);
            entityProperty = null;

            var valueType = Value.GetType();
            if (PrimitiveTypes.TryGetValue(valueType, out var factory))
            {
                entityProperty = factory(Value);
                return true;
            }

            if (valueType.IsEnum)
            {
                entityProperty = new EntityProperty(Value.ToString());
                return true;
            }

            if (CanSerializeWithoutNewtonsoftJson(Value))
            {
                return false;
            }

            var serializedValue = SerializeToJson(Value);
            var serializedValueSize = Encoding.Unicode.GetByteCount(serializedValue);
            if (serializedValueSize > MaxTableCellSizeBytes)
            {
                serializedValue = SerializeToGZip(serializedValue);
                columnName += GzipPropertySuffix;
            }
            else
            {
                columnName += NewtonsoftJsonPropertySuffix;
            }

            entityProperty = new EntityProperty(serializedValue);
            return true;
        }

        /// <summary>
        /// Sets value to <paramref name="property"/>.
        /// </summary>
        /// <param name="property">Property to be set.</param>
        /// <param name="owner">Object that owns property.</param>
        public void SetTo(PropertyInfo property, object owner)
        {
            if (property == null)
            {
                return;
            }

            if (Format == AzureTableValueFormat.Json)
            {
                var jsonValue = DeserializeFromJson((string)Value, property.PropertyType);
                property.SetValue(owner, jsonValue);
                return;
            }

            if (Format == AzureTableValueFormat.GZip)
            {
                var gzipValue = DeserializeFromGZip((string)Value);
                var jsonValue = DeserializeFromJson(gzipValue, property.PropertyType);
                property.SetValue(owner, jsonValue);
                return;
            }

            property.SetValue(owner, ConvertToPropertyType(Value, property.PropertyType));
        }

        /// <summary>
        /// Selects child properties values of <see cref="Value"/> object.
        /// </summary>
        /// <returns>Sequence of child properties.</returns>
        public IEnumerable<AzureTableValue> SelectChildValues()
        {
            var type = Value.GetType();
            var properties = type.GetProperties();

            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.Name.Contains(DefaultPropertyNameDelimiter))
                {
                    throw new SerializationException($"Property '{propertyInfo.Name}' already contains property name delimiter '{DefaultPropertyNameDelimiter}' in its name.");
                }

                if (ShouldSkip(propertyInfo))
                {
                    continue;
                }

                var childPath = Path.Add(propertyInfo.Name);
                var childValue = propertyInfo.GetValue(Value, null);

                yield return new AzureTableValue(childPath, childValue);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{string.Join(DefaultPropertyNameDelimiter, Path)}={Value}";
        }

        private static bool ShouldSkip(PropertyInfo propertyInfo)
        {
            return !propertyInfo.CanWrite
                || !propertyInfo.CanRead
                || Attribute.IsDefined(propertyInfo, typeof(IgnorePropertyAttribute))
                || propertyInfo.Name.Equals("ETag", StringComparison.Ordinal)
                || propertyInfo.Name.Equals("Timestamp", StringComparison.Ordinal);
        }

        private static bool CanSerializeWithoutNewtonsoftJson(object value)
        {
            if (value is IEnumerable)
            {
                return false;
            }

            var constructors = value.GetType().GetConstructors();

            // No public ctor.
            if (constructors.Length == 0)
            {
                return false;
            }

            // All ctors have parameters.
            if (constructors.All(c => c.GetParameters().Length > 0))
            {
                return false;
            }

            // Have ctor with [JsonConstructor] attribute.
            if (constructors.Any(c => c.CustomAttributes.Any(ca => ca.AttributeType == typeof(JsonConstructorAttribute))))
            {
                return false;
            }

            return true;
        }

        private static object ConvertToPropertyType(object value, Type propertyType)
        {
            var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            if (underlyingType.IsEnum)
            {
                return Enum.Parse(underlyingType, value.ToString());
            }

            if (value is DateTime dateTime && dateTime == AzureTableStorageMinDateTime)
            {
                value = new DateTime(0, DateTimeKind.Utc);
            }

            if (underlyingType == typeof(DateTimeOffset))
            {
                return new DateTimeOffset((DateTime)value);
            }

            if (underlyingType == typeof(TimeSpan))
            {
                return TimeSpan.Parse(value.ToString(), CultureInfo.InvariantCulture);
            }

            if (underlyingType == typeof(uint))
            {
                return (uint)(long)value;
            }

            if (underlyingType == typeof(ulong))
            {
                return (ulong)(long)value;
            }

            if (underlyingType == typeof(byte))
            {
                return ((byte[])value)[0];
            }

            return Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);
        }

        private static string SerializeToJson(object value)
        {
            return JsonConvert.SerializeObject(value, JsonSerializerSettings);
        }

        private static object DeserializeFromJson(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type, JsonSerializerSettings);
        }

        private static string SerializeToGZip(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);

            using var msi = new MemoryStream(bytes);
            using var mso = new MemoryStream();
            using (var gs = new GZipStream(mso, CompressionMode.Compress))
            {
                msi.CopyTo(gs);
            }

            return Convert.ToBase64String(mso.ToArray());
        }

        private static string DeserializeFromGZip(string value)
        {
            using var msi = new MemoryStream(Convert.FromBase64String(value));
            using var mso = new MemoryStream();
            using (var gs = new GZipStream(msi, CompressionMode.Decompress))
            {
                gs.CopyTo(mso);
            }

            return Encoding.UTF8.GetString(mso.ToArray());
        }
    }
}