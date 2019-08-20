using System;
using System.Globalization;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Tests.Utils
{
    public class TestsExecutionContext
    {
        private const string Prefix = nameof(CQRS);
        private static readonly string Timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMddhhmmssf", CultureInfo.InvariantCulture);

        public string GetSessionPrefix()
            => $"{Prefix}{Timestamp}";

        public string GetUniqueStorageTablePrefix()
            => $"{this.GetSessionPrefix()}{UniqueToken()}";

        private static string UniqueToken()
            => Unified.NewCode();
    }
}