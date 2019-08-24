using System;
using FluentAssertions;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.QueryStore.AzureTable.Common;
using Xunit;

namespace Intellias.CQRS.Tests.QueryStores
{
    public class BaseJsonTableEntityTests
    {
        [Fact]
        public void Serialization_DefaultValue_DeserializedCorrectly()
        {
            var data = new DummyData();

            var entity = new DummyJsonTableEntity(data);

            entity.DeserializeData().Should().BeEquivalentTo(data);
        }

        [Fact]
        public void Serialization_PropertyIsImmutable_UpdatesValue()
        {
            var data = new DummyData { SnapshotId = new SnapshotId(Unified.NewCode(), 1) };

            var entity = new DummyJsonTableEntity(data);

            entity.DeserializeData().Should().BeEquivalentTo(data);
        }

        [Fact]
        public void Serialization_PropertyIsDateTimeOffset_KeepsOffsetValue()
        {
            var data = new DummyData { DateTimeOffset = DateTimeOffset.Now }; // Initialize value with non-UTC.

            var entity = new DummyJsonTableEntity(data);

            entity.DeserializeData().Should().BeEquivalentTo(data);
        }

        [Fact]
        public void Serialization_PropertyIsMissing_LeavesExistingValue()
        {
            var dataWithoutProperty = new DummyDataWithoutSnapshotId { DateTimeOffset = DateTimeOffset.UtcNow.AddDays(1) };
            var expected = new DummyData { DateTimeOffset = dataWithoutProperty.DateTimeOffset };

            // Create JSON without property.
            var entityWithoutProperty = new DummyWithoutSnapshotIdJsonTableEntity(dataWithoutProperty);

            // Set JSON to entity with property.
            var entity = new DummyJsonTableEntity { Data = entityWithoutProperty.Data };

            entity.DeserializeData().Should().BeEquivalentTo(expected);
        }

        private class DummyJsonTableEntity : BaseJsonTableEntity<DummyData>
        {
            public DummyJsonTableEntity()
            {
            }

            public DummyJsonTableEntity(DummyData data)
                : base(data)
            {
            }

            protected override void SetupDeserializedData(DummyData data)
            {
            }
        }

        private class DummyWithoutSnapshotIdJsonTableEntity : BaseJsonTableEntity<DummyDataWithoutSnapshotId>
        {
            public DummyWithoutSnapshotIdJsonTableEntity(DummyDataWithoutSnapshotId data)
                : base(data)
            {
            }

            protected override void SetupDeserializedData(DummyDataWithoutSnapshotId data)
            {
            }
        }

        private class DummyDataWithoutSnapshotId
        {
            public DateTimeOffset DateTimeOffset { get; set; } = DateTimeOffset.UtcNow;
        }

        private class DummyData
        {
            public DateTimeOffset DateTimeOffset { get; set; } = DateTimeOffset.UtcNow;

            public SnapshotId SnapshotId { get; set; } = SnapshotId.Empty;
        }
    }
}