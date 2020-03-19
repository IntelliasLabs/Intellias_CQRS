using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Microsoft.Azure.Cosmos.Table;

namespace Intellias.CQRS.Persistence.AzureStorage.Common
{
    /// <summary>
    /// Serializes object into table.
    /// </summary>
    public static class AzureTableSerializer
    {
        /// <summary>
        /// Serialized type name column name.
        /// </summary>
        public const string TypeNameColumnName = "_TypeName";

        /// <summary>
        /// Serializes object into table presentation.
        /// </summary>
        /// <param name="root">Object to be serialized.</param>
        /// <param name="persistType">Save type of serialized object.</param>
        /// <returns>Set of column name/value pairs.</returns>
        public static Dictionary<string, EntityProperty> Serialize(object root, bool persistType = false)
        {
            var dict = new Dictionary<string, EntityProperty>();

            if (root == null)
            {
                return dict;
            }

            var antecedents = new HashSet<object>(new ObjectReferenceEqualityComparer());
            var tableValue = new AzureTableValue(ImmutableList<string>.Empty, root);
            if (!Serialize(dict, antecedents, tableValue))
            {
                throw new InvalidOperationException($"Can not flatten object of type '{root.GetType()}'.");
            }

            if (persistType)
            {
                var typeName = root.GetType().AssemblyQualifiedName;
                dict.Add(TypeNameColumnName, new EntityProperty(typeName));
            }

            return dict;
        }

        /// <summary>
        /// Deserializes table entity into object.
        /// </summary>
        /// <param name="entity">Table entity.</param>
        /// <typeparam name="T">Object type.</typeparam>
        /// <returns>Object.</returns>
        public static T Deserialize<T>(DynamicTableEntity entity)
            where T : new()
        {
            if (entity.Properties == null)
            {
                return new T();
            }

            var uninitializedObject = FormatterServices.GetUninitializedObject(typeof(T));
            var result = entity.Properties
                .Select(kvp => AzureTableValue.Create(kvp.Key, kvp.Value.PropertyAsObject))
                .Aggregate(uninitializedObject, SetProperty);

            return (T)result;
        }

        /// <summary>
        /// Deserializes table entity into object.
        /// </summary>
        /// <param name="entity">Table entity.</param>
        /// <returns>Object.</returns>
        public static object Deserialize(DynamicTableEntity entity)
        {
            if (entity.Properties == null)
            {
                return new object();
            }

            if (!entity.Properties.TryGetValue(TypeNameColumnName, out var typeNameProperty)
                || !(typeNameProperty.PropertyAsObject is string typeNamePropertyValue))
            {
                throw new InvalidOperationException($"Unable to get object type name. Table entity either missing '{TypeNameColumnName}' column or it's empty.");
            }

            var objectType = Type.GetType(typeNamePropertyValue);
            if (objectType == null)
            {
                throw new TypeLoadException($"Unable to resolve table entity type '{typeNamePropertyValue}'.");
            }

            var uninitializedObject = FormatterServices.GetUninitializedObject(objectType);
            var result = entity.Properties
                .Select(kvp => AzureTableValue.Create(kvp.Key, kvp.Value.PropertyAsObject))
                .Aggregate(uninitializedObject, SetProperty);

            return result;
        }

        private static bool Serialize(Dictionary<string, EntityProperty> propertyDictionary, ISet<object> antecedents, AzureTableValue value)
        {
            if (value.Value == null)
            {
                return true;
            }

            if (value.TrySerialize(out var columnName, out var propertyEntity))
            {
                propertyDictionary.Add(columnName, propertyEntity);
                return true;
            }

            if (!value.Type.IsValueType && !antecedents.Add(value.Value))
            {
                throw new SerializationException($"Circular reference of value '{value}' of type '{value.Value.GetType()}' is detected.");
            }

            var successful = value
                .SelectChildValues()
                .All(v => Serialize(propertyDictionary, antecedents, v));

            antecedents.Remove(value.Value);

            return successful;
        }

        private static object SetProperty(object rootObject, AzureTableValue value)
        {
            try
            {
                var valueTypeProperties = new Stack<ValueTypeProperty>();

                var childObject = rootObject;
                var hasValueTypes = false;
                for (var index = 0; index < value.Path.Count - 1; ++index)
                {
                    var propertyInfo = childObject.GetType().GetProperty(value.Path[index]);
                    if (propertyInfo == null)
                    {
                        continue;
                    }

                    var childObjectPropertyValue = propertyInfo.GetValue(childObject, null);

                    // Value Type is never NULL so will be skipped.
                    if (childObjectPropertyValue == null)
                    {
                        childObjectPropertyValue = FormatterServices.GetUninitializedObject(propertyInfo.PropertyType);
                        propertyInfo.SetValue(childObject, childObjectPropertyValue, null);
                    }

                    // In case of Value Type record in stack to reassign later.
                    if (hasValueTypes || propertyInfo.PropertyType.IsValueType)
                    {
                        hasValueTypes = true;
                        valueTypeProperties.Push(new ValueTypeProperty(propertyInfo, childObject));
                    }

                    childObject = childObjectPropertyValue;
                }

                var prop = childObject.GetType().GetProperty(value.Path.Last());

                value.SetTo(prop, childObject);

                // Reassign Value Types values encountered in path.
                while (valueTypeProperties.Count > 0)
                {
                    var valueTypeProperty = valueTypeProperties.Pop();

                    valueTypeProperty.Info.SetValue(valueTypeProperty.Owner, childObject, null);

                    childObject = valueTypeProperty.Owner;
                }

                return rootObject;
            }
            catch (Exception exception)
            {
                throw new SerializationException($"Exception thrown while trying to assign value '{value}'.", exception);
            }
        }

        private class ObjectReferenceEqualityComparer : IEqualityComparer<object>
        {
            bool IEqualityComparer<object>.Equals(object x, object y)
            {
                return x == y;
            }

            public int GetHashCode(object obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }

        private class ValueTypeProperty
        {
            public ValueTypeProperty(PropertyInfo info, object owner)
            {
                Info = info;
                Owner = owner;
            }

            public PropertyInfo Info { get; }

            public object Owner { get; }
        }
    }
}