using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoFixture;
using FluentAssertions;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Intellias.CQRS.Tests.Utils;
using Microsoft.Azure.Cosmos.Table;
using Xunit;

namespace Intellias.CQRS.Tests.Persistence.AzureStorage.Common
{
    public class AzureTableSerializerTests
    {
        private readonly Fixture fixture;

        public AzureTableSerializerTests()
        {
            fixture = new Fixture();
            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(fixture);
        }

        [Fact]
        public void Serialize_NullObject_ReturnsEmptyResult()
        {
            var result = AzureTableSerializer.Serialize(null);

            result.Should().BeEmpty();
        }

        [Fact]
        public void Serialize_CircularDependency_Throws()
        {
            var source = new CircularDependencyObject();
            source.DependencyProperty = source;

            source.Invoking(s => AzureTableSerializer.Serialize(s)).Should().Throw<SerializationException>()
                .And.Message.Should().Contain("Circular");
        }

        [Fact]
        public void Serialize_ForbiddenPropertyName_Throws()
        {
            var source = fixture.Create<ForbiddenPropertyNameObject>();

            source.Invoking(s => AzureTableSerializer.Serialize(s)).Should().Throw<SerializationException>()
                .And.Message.Should().Contain(nameof(ForbiddenPropertyNameObject.ForbiddenName_Property));
        }

        [Fact]
        public void Serialize_PersistType_AddsTypeNameColumn()
        {
            var source = fixture.Create<ZeroDepthObject>();

            var serialized = AzureTableSerializer.Serialize(source, true);

            serialized[AzureTableSerializer.TypeNameColumnName].PropertyAsObject.Should()
                .Be(source.GetType().AssemblyQualifiedName);
        }

        [Fact]
        public void Deserialize_ExceptionPropertyObject_Throws()
        {
            var source = new ExceptionPropertyObject();
            var serialized = AzureTableSerializer.Serialize(source);

            serialized.Invoking(s => AzureTableSerializer.Deserialize<ExceptionPropertyObject>(Entity(s)))
                .Should().Throw<SerializationException>()
                .And.Message.Contains(nameof(ExceptionPropertyObject.ExceptionProperty), StringComparison.Ordinal);
        }

        [Fact]
        public void Deserialize_ZeroDepthObject_WorksCorrectly()
        {
            var source = fixture.Create<ZeroDepthObject>();

            var serialized = AzureTableSerializer.Serialize(source);
            var deserialized = AzureTableSerializer.Deserialize<ZeroDepthObject>(Entity(serialized));

            deserialized.Should().BeEquivalentTo(source, options => options
                .Excluding(o => o.Timestamp));
        }

        [Fact]
        public void Deserialize_MultilevelObject_WorksCorrectly()
        {
            var source = fixture.Create<MultilevelObject>();

            var serialized = AzureTableSerializer.Serialize(source);
            var deserialized = AzureTableSerializer.Deserialize<MultilevelObject>(Entity(serialized));

            deserialized.Should().BeEquivalentTo(source, options => options
                .Excluding(o => o.Timestamp));
        }

        [Fact]
        public void Deserialize_StructureObject_WorksCorrectly()
        {
            var source = fixture.Create<StructureObject>();

            var serialized = AzureTableSerializer.Serialize(source);
            var deserialized = AzureTableSerializer.Deserialize<StructureObject>(Entity(serialized));

            deserialized.Should().BeEquivalentTo(source, options => options
                .Excluding(o => o.Timestamp));
        }

        [Fact]
        public void Deserialize_CollectionObject_WorksCorrectly()
        {
            var source = fixture.Create<CollectionObject>();

            var serialized = AzureTableSerializer.Serialize(source);
            var deserialized = AzureTableSerializer.Deserialize<CollectionObject>(Entity(serialized));

            deserialized.Should().BeEquivalentTo(source, options => options
                .Excluding(o => o.Timestamp));
        }

        [Fact]
        public void Deserialize_DictionaryObject_WorksCorrectly()
        {
            var source = fixture.Create<DictionaryObject>();

            var serialized = AzureTableSerializer.Serialize(source);
            var deserialized = AzureTableSerializer.Deserialize<DictionaryObject>(Entity(serialized));

            deserialized.Should().BeEquivalentTo(source, options => options
                .Excluding(o => o.Timestamp));
        }

        [Fact(Skip = "Skip until POC code is moved to CQRS and AppliedEvent/SnapshotId are made anemic.")]
        public void Deserialize_ImmutablePropertiesObject_WorksCorrectly()
        {
            var source = fixture.Create<ImmutablePropertiesObject>();

            var serialized = AzureTableSerializer.Serialize(source);
            var deserialized = AzureTableSerializer.Deserialize<ImmutablePropertiesObject>(Entity(serialized));

            deserialized.Should().BeEquivalentTo(source, options => options
                .Excluding(o => o.Timestamp));
        }

        [Fact]
        public void Deserialize_ObjectHasNoPropertyWithSuchName_WorksCorrectly()
        {
            var source = fixture.Create<ZeroDepthObject>();

            var serialized = AzureTableSerializer.Serialize(source);
            serialized["SomeRandomPropertyName"] = new EntityProperty("SomeRandomPropertyValue");

            var deserialized = AzureTableSerializer.Deserialize<ZeroDepthObject>(Entity(serialized));

            deserialized.Should().BeEquivalentTo(source, options => options
                .Excluding(o => o.Timestamp));
        }

        [Fact]
        public void DeserializeWithoutType_NoTypeNameColumn_WorksCorrectly()
        {
            var source = fixture.Create<ZeroDepthObject>();
            var serialized = AzureTableSerializer.Serialize(source);

            serialized.Invoking(s => AzureTableSerializer.Deserialize(Entity(s)))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void DeserializeWithoutType_TypeNameIsUnknown_WorksCorrectly()
        {
            var source = fixture.Create<ZeroDepthObject>();
            var serialized = AzureTableSerializer.Serialize(source, true);

            // Replace existing type name with unknown one.
            var serializedUnknownTypeName = serialized[AzureTableSerializer.TypeNameColumnName].StringValue
                .Replace(nameof(ZeroDepthObject), nameof(ZeroDepthObject) + "2", StringComparison.Ordinal);
            serialized[AzureTableSerializer.TypeNameColumnName] = new EntityProperty(serializedUnknownTypeName);

            serialized.Invoking(s => AzureTableSerializer.Deserialize(Entity(s)))
                .Should().Throw<TypeLoadException>();
        }

        [Fact]
        public void DeserializeWithoutType_ZeroDepthObject_WorksCorrectly()
        {
            var source = fixture.Create<ZeroDepthObject>();

            var serialized = AzureTableSerializer.Serialize(source, true);
            var deserialized = AzureTableSerializer.Deserialize(Entity(serialized));

            deserialized.Should().BeEquivalentTo(source, options => options
                .Excluding(o => o.Timestamp));
        }

        [Fact]
        public void DeserializeWithoutType_CollectionObject_WorksCorrectly()
        {
            var source = fixture.Create<CollectionObject>();

            var serialized = AzureTableSerializer.Serialize(source, true);
            var deserialized = AzureTableSerializer.Deserialize(Entity(serialized));

            deserialized.Should().BeEquivalentTo(source, options => options
                .Excluding(o => o.Timestamp));
        }

        [Fact]
        public void Serialize_MinDateTimeObject_SetsToMinValuesForTableStorage()
        {
            var source = new MinDateTimeObject();

            var serialized = AzureTableSerializer.Serialize(source);

            serialized[nameof(source.DateTimeProperty)].PropertyAsObject.Should().BeEquivalentTo(new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            serialized[nameof(source.DateTimeOffsetProperty)].PropertyAsObject.Should().BeEquivalentTo(new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        [Fact]
        public void Deserialize_MinDateTimeObject_RestoredDefaultValue()
        {
            var source = new MinDateTimeObject();

            var serialized = AzureTableSerializer.Serialize(source);
            var deserialized = AzureTableSerializer.Deserialize<MinDateTimeObject>(Entity(serialized));

            deserialized.Should().BeEquivalentTo(source, options => options
                .Excluding(o => o.Timestamp));
        }

        [Fact]
        public void Deserialize_HugeCollection_UsesGZip()
        {
            var source = new CollectionObject
            {
                RandomObject1CollectionProperty = FixtureUtils.Array(1000, 1000, () => new RandomObject1
                {
                    StringProperty = FixtureUtils.String()
                }).ToList()
            };

            var serialized = AzureTableSerializer.Serialize(source);
            var deserialized = AzureTableSerializer.Deserialize<CollectionObject>(Entity(serialized));

            serialized.Keys.Any(k => k.Contains("GZip", StringComparison.Ordinal)).Should().BeTrue();
            deserialized.Should().BeEquivalentTo(source, options => options
                .Excluding(o => o.Timestamp));
        }

        private static DynamicTableEntity Entity(Dictionary<string, EntityProperty> properties)
        {
            return new DynamicTableEntity
            {
                Properties = properties
            };
        }

        private struct Structure1Object
        {
            public string String1Property { get; set; }

            public string String2Property { get; set; }

            public Structure2Object Structure1ObjectProperty { get; set; }
        }

        private struct Structure2Object
        {
            public string String1Property { get; set; }

            public string String2Property { get; set; }
        }

        private class MinDateTimeObject : TestObject
        {
            public DateTime DateTimeProperty { get; set; }

            public DateTimeOffset DateTimeOffsetProperty { get; set; }
        }

        private class StructureObject : TestObject
        {
            public Structure1Object Structure1ObjectProperty { get; set; }
        }

        private class ImmutablePropertiesObject : TestObject
        {
            public SnapshotId SnapshotIdProperty { get; set; }

            public AppliedEvent AppliedEventProperty { get; set; }
        }

        private class CollectionObject : TestObject
        {
            public List<RandomObject1> RandomObject1CollectionProperty { get; set; }

            public List<RandomObject2> RandomObject2CollectionProperty { get; set; }

            public List<string> StringCollectionProperty { get; set; }
        }

        private class DictionaryObject : TestObject
        {
            public Dictionary<string, RandomObject1> RandomObject1DictionaryProperty { get; set; }
        }

        private class MultilevelObject : TestObject
        {
            public string StringProperty { get; set; }

            public RandomObject1 RandomObject1Property { get; set; }
        }

        private class RandomObject1
        {
            public string StringProperty { get; set; }

            public RandomObject2 RandomObject2Property { get; set; }
        }

        private class RandomObject2
        {
            public string StringProperty { get; set; }
        }

        private class CircularDependencyObject : TestObject
        {
            public CircularDependencyObject DependencyProperty { get; set; }
        }

        private class ForbiddenPropertyNameObject : TestObject
        {
            public string ForbiddenName_Property { get; set; }
        }

        private class ExceptionPropertyObject : TestObject
        {
            public string ExceptionProperty
            {
                get => "1";
                set => throw new InvalidOperationException("Unable to set property!");
            }
        }

        private class ZeroDepthObject : TestObject
        {
            public string StringProperty { get; set; }

            public byte[] ByteArrayProperty { get; set; }

            public byte ByteProperty { get; set; }

            public bool BoolProperty { get; set; }

            public bool? NullableBoolProperty { get; set; }

            public DateTime DateTimeProperty { get; set; }

            public DateTime? NullableDateTimeProperty { get; set; }

            public DateTimeOffset DateTimeOffsetProperty { get; set; }

            public DateTimeOffset? NullableDateTimeOffsetProperty { get; set; }

            public double DoubleProperty { get; set; }

            public double? NullableDoubleProperty { get; set; }

            public Guid GuidProperty { get; set; }

            public Guid? NullableGuidProperty { get; set; }

            public int IntProperty { get; set; }

            public int? NullableIntProperty { get; set; }

            public uint UintProperty { get; set; }

            public uint? NullableUintProperty { get; set; }

            public long LongProperty { get; set; }

            public long? NullableLongProperty { get; set; }

            public ulong UlongProperty { get; set; }

            public ulong? NullableUlongProperty { get; set; }

            public TimeSpan TimeSpanProperty { get; set; }

            public TimeSpan? NullableTimeSpanProperty { get; set; }
        }

        private abstract class TestObject : IQueryModel
        {
            public string Id { get; set; }

            public int Version { get; set; }

            public DateTime Timestamp { get; set; }
        }
    }
}