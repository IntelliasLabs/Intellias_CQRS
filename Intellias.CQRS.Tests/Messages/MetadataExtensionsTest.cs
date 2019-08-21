using FluentAssertions;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Events;
using Xunit;

namespace Intellias.CQRS.Tests.Messages
{
    public class MetadataExtensionsTest
    {
        [Fact]
        public void CopyMetadataFromSuccessTest()
        {
            var cmd = new TestCreateCommand();
            cmd.Metadata.Add(MetadataKey.AgreegateType, "Test");
            cmd.Metadata.Add(MetadataKey.UserId, Unified.NewCode());

            var ev = new TestCreatedEvent();
            cmd.CopyMetadata(ev);

            ev.TypeName.Should().Be(typeof(TestCreatedEvent).AssemblyQualifiedName);
            ev.Metadata[MetadataKey.AgreegateType].Should().Be(cmd.Metadata[MetadataKey.AgreegateType]);
            ev.Metadata[MetadataKey.UserId].Should().Be(cmd.Metadata[MetadataKey.UserId]);
            ev.Metadata.TryGetValue(MetadataKey.RequestHeaders, out _).Should().BeFalse();
            ev.Metadata.TryGetValue(MetadataKey.Roles, out _).Should().BeFalse();
        }

        [Fact]
        public void CopyMetadataFromWhereMetadataContainsDataNotFromMetadataKeyEnum()
        {
            var cmd = new TestCreateCommand();
            cmd.Metadata.Add((MetadataKey)111, "SomeCustomData");

            var ev = new TestCreatedEvent();
            cmd.CopyMetadata(ev);

            ev.Metadata[(MetadataKey)111].Should().Be("SomeCustomData");
        }
    }
}
