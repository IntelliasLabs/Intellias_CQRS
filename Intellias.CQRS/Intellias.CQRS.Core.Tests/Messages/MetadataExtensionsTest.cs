using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.Events;
using Xunit;

namespace Intellias.CQRS.Core.Tests.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class MetadataExtensionsTest
    {
        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void CopyMetadataFromSuccessTest()
        {
            var cmd = new TestCreateCommand();
            cmd.Metadata.Add(MetadataKey.AgreegateType, "Test");
            cmd.Metadata.Add(MetadataKey.UserId, Unified.NewCode());

            var ev = new TestCreatedEvent();

            ev.CopyMetadataFrom(cmd);

            ev.Metadata[MetadataKey.TypeName].Should().Be(typeof(TestCreatedEvent).Name);
            ev.Metadata[MetadataKey.AgreegateType].Should().Be(cmd.Metadata[MetadataKey.AgreegateType]);
            ev.Metadata[MetadataKey.UserId].Should().Be(cmd.Metadata[MetadataKey.UserId]);
            ev.Metadata.TryGetValue(MetadataKey.RequestHeaders, out var rh).Should().BeFalse();
            ev.Metadata.TryGetValue(MetadataKey.Roles, out var roles).Should().BeFalse();
        }

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void CopyMetadataFromWhereMetadataContainsDataNotFromMetadataKeyEnum()
        {
            var cmd = new TestCreateCommand();
            cmd.Metadata.Add((MetadataKey)111, "SomeCustomData");

            var ev = new TestCreatedEvent();

            ev.CopyMetadataFrom(cmd);

            ev.Metadata[(MetadataKey)111].Should().Be("SomeCustomData");
        }
    }
}
