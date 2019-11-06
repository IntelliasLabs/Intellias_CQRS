// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Intellias.CQRS.Core.DataAnnotations.Validators
{
    /// <summary>
    /// Cache of <see cref="ValidationAttribute" />.
    /// </summary>
    internal class CoreValidationAttributeStore
    {
        private readonly Dictionary<Type, TypeStoreItem> typeStoreItems = new Dictionary<Type, TypeStoreItem>();

        /// <summary>
        /// Gets the singleton <see cref="CoreValidationAttributeStore" />.
        /// </summary>
        internal static CoreValidationAttributeStore Instance { get; } = new CoreValidationAttributeStore();

        /// <summary>
        /// Checks whether property is public.
        /// </summary>
        /// <param name="propertyInfo">Property info.</param>
        /// <returns>True if property is public.</returns>
        internal static bool IsPublic(PropertyInfo propertyInfo)
        {
            return (propertyInfo.GetMethod != null && propertyInfo.GetMethod.IsPublic) || (propertyInfo.SetMethod != null && propertyInfo.SetMethod.IsPublic);
        }

        /// <summary>
        /// Checks whether property is static.
        /// </summary>
        /// <param name="propertyInfo">Property info.</param>
        /// <returns>True if property is static.</returns>
        internal static bool IsStatic(PropertyInfo propertyInfo)
        {
            return (propertyInfo.GetMethod != null && propertyInfo.GetMethod.IsStatic) || (propertyInfo.SetMethod != null && propertyInfo.SetMethod.IsStatic);
        }

        /// <summary>
        /// Retrieves the type level validation attributes for the given type.
        /// </summary>
        /// <param name="validationContext">The context that describes the type. It cannot be null.</param>
        /// <returns>The collection of validation attributes. It could be empty.</returns>
        internal IReadOnlyCollection<ValidationAttribute> GetTypeValidationAttributes(ValidationContext validationContext)
        {
            var item = GetTypeStoreItem(validationContext.ObjectType);
            return item.ValidationAttributes;
        }

        /// <summary>
        /// Retrieves the <see cref="DisplayAttribute" /> associated with the given type. It may be null.
        /// </summary>
        /// <param name="validationContext">The context that describes the type. It cannot be null.</param>
        /// <returns>The display attribute instance, if present.</returns>
        internal DisplayAttribute GetTypeDisplayAttribute(ValidationContext validationContext)
        {
            var item = GetTypeStoreItem(validationContext.ObjectType);
            return item.DisplayAttribute;
        }

        /// <summary>
        /// Retrieves the set of validation attributes for the property.
        /// </summary>
        /// <param name="validationContext">The context that describes the property. It cannot be null.</param>
        /// <returns>The collection of validation attributes. It could be empty.</returns>
        internal IReadOnlyCollection<ValidationAttribute> GetPropertyValidationAttributes(ValidationContext validationContext)
        {
            var typeItem = GetTypeStoreItem(validationContext.ObjectType);
            var item = typeItem.GetPropertyStoreItem(validationContext.MemberName);
            return item.ValidationAttributes;
        }

        /// <summary>
        /// Retrieves the <see cref="DisplayAttribute" /> associated with the given property.
        /// </summary>
        /// <param name="validationContext">The context that describes the property. It cannot be null.</param>
        /// <returns>The display attribute instance, if present.</returns>
        internal DisplayAttribute GetPropertyDisplayAttribute(ValidationContext validationContext)
        {
            var typeItem = GetTypeStoreItem(validationContext.ObjectType);
            var item = typeItem.GetPropertyStoreItem(validationContext.MemberName);
            return item.DisplayAttribute;
        }

        /// <summary>
        /// Retrieves the Type of the given property.
        /// </summary>
        /// <param name="validationContext">The context that describes the property. It cannot be null.</param>
        /// <returns>The type of the specified property.</returns>
        internal Type GetPropertyType(ValidationContext validationContext)
        {
            var typeItem = GetTypeStoreItem(validationContext.ObjectType);
            var item = typeItem.GetPropertyStoreItem(validationContext.MemberName);
            return item.PropertyType;
        }

        /// <summary>
        /// Determines whether or not a given <see cref="ValidationContext" />'s
        /// <see cref="ValidationContext.MemberName" /> references a property on
        /// the <see cref="ValidationContext.ObjectType" />.
        /// </summary>
        /// <param name="validationContext">The <see cref="ValidationContext" /> to check.</param>
        /// <returns><c>true</c> when the <paramref name="validationContext" /> represents a property, <c>false</c> otherwise.</returns>
        internal bool IsPropertyContext(ValidationContext validationContext)
        {
            var typeItem = GetTypeStoreItem(validationContext.ObjectType);
            return typeItem.TryGetPropertyStoreItem(validationContext.MemberName, out _);
        }

        /// <summary>
        /// Retrieves or creates the store item for the given type.
        /// </summary>
        /// <param name="type">The type whose store item is needed. It cannot be null.</param>
        /// <returns>The type store item. It will not be null.</returns>
        private TypeStoreItem GetTypeStoreItem(Type type)
        {
            lock (typeStoreItems)
            {
                if (typeStoreItems.TryGetValue(type, out var item))
                {
                    return item;
                }

                // use CustomAttributeExtensions.GetCustomAttributes() to get inherited attributes as well as direct ones
                var attributes = CustomAttributeExtensions.GetCustomAttributes(type, true).ToArray();
                item = new TypeStoreItem(type, attributes);
                typeStoreItems[type] = item;

                return item;
            }
        }

        /// <summary>
        /// Private abstract class for all store items.
        /// </summary>
        private abstract class StoreItem
        {
            protected StoreItem(IReadOnlyCollection<Attribute> attributes)
            {
                ValidationAttributes = attributes.OfType<ValidationAttribute>().ToArray();
                DisplayAttribute = attributes.OfType<DisplayAttribute>().SingleOrDefault();
            }

            internal IReadOnlyCollection<ValidationAttribute> ValidationAttributes { get; }

            internal DisplayAttribute DisplayAttribute { get; }
        }

        /// <summary>
        /// Private class to store data associated with a type.
        /// </summary>
        private class TypeStoreItem : StoreItem
        {
            private readonly object syncRoot = new object();
            private readonly Type type;
            private Dictionary<string, PropertyStoreItem> propertyStoreItems;

            internal TypeStoreItem(Type type, IReadOnlyCollection<Attribute> attributes)
                : base(attributes)
            {
                this.type = type;
            }

            internal PropertyStoreItem GetPropertyStoreItem(string propertyName)
            {
                if (!TryGetPropertyStoreItem(propertyName, out var item))
                {
                    throw new ArgumentException($"The type '{type.Name}' does not contain a public property named '{propertyName}'.", nameof(propertyName));
                }

                return item;
            }

            internal bool TryGetPropertyStoreItem(string propertyName, out PropertyStoreItem item)
            {
                if (string.IsNullOrEmpty(propertyName))
                {
                    throw new ArgumentNullException(nameof(propertyName));
                }

                if (propertyStoreItems != null)
                {
                    return propertyStoreItems.TryGetValue(propertyName, out item);
                }

                lock (syncRoot)
                {
                    if (propertyStoreItems == null)
                    {
                        propertyStoreItems = CreatePropertyStoreItems();
                    }
                }

                return propertyStoreItems.TryGetValue(propertyName, out item);
            }

            private Dictionary<string, PropertyStoreItem> CreatePropertyStoreItems()
            {
                var createdItems = new Dictionary<string, PropertyStoreItem>();

                // exclude index properties to match old TypeDescriptor functionality
                var properties = type.GetRuntimeProperties()
                    .Where(prop => IsPublic(prop) && !prop.GetIndexParameters().Any());

                foreach (var property in properties)
                {
                    // use CustomAttributeExtensions.GetCustomAttributes() to get inherited attributes as well as direct ones
                    var customAttributes = CustomAttributeExtensions.GetCustomAttributes(property, true).ToArray();
                    var item = new PropertyStoreItem(property.PropertyType, customAttributes);

                    createdItems[property.Name] = item;
                }

                return createdItems;
            }
        }

        /// <summary>
        /// Private class to store data associated with a property.
        /// </summary>
        private class PropertyStoreItem : StoreItem
        {
            internal PropertyStoreItem(Type propertyType, IReadOnlyCollection<Attribute> attributes)
                : base(attributes)
            {
                PropertyType = propertyType;
            }

            internal Type PropertyType { get; }
        }
    }
}
