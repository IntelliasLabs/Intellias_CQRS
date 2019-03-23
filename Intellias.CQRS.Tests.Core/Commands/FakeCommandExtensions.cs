using System;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Tests.Core.Commands
{
    /// <summary>
    /// Fake extensions for commands
    /// </summary>
    public static class FakeCommandExtensions
    {
        /// <summary>
        /// Wraps command with some defaults properies like Id, CorrelationId
        /// and metadata fields UserId, Roles to pass validation
        /// </summary>
        /// <param name="cmd"></param>
        public static void Wrap(this Command cmd)
        {
            cmd.Id = Unified.NewCode();
            cmd.CorrelationId = Unified.NewCode();
            cmd.Metadata[MetadataKey.UserId] = Guid.NewGuid().ToString();
            cmd.Metadata[MetadataKey.Roles] = "Admin";
        }
    }
}
