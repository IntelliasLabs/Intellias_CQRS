using System.Text.RegularExpressions;
using FluentAssertions;
using Intellias.CQRS.Core.DataAnnotations;
using Xunit;

namespace Intellias.CQRS.Tests.Core.DataAnnotations
{
    public class RegularExpressionsTests
    {
        [Theory]
        [InlineData("abc123_", true)]
        [InlineData("_123abc", true)]
        [InlineData("abc-", false)]
        [InlineData("#", false)]
        [InlineData("abc abc", false)]
        [InlineData("кирилица", false)]
        public void Match_AlphaNumeric_IsCorrect(string input, bool isMatch)
        {
            Regex.IsMatch(input, RegularExpressions.Alphanumeric).Should().Be(isMatch);
        }

        [Theory]
        [InlineData("C#", true)]
        [InlineData("ASP .NET Core", true)]
        [InlineData("C++", true)]
        [InlineData("abcezAABDSCXZ0-9_-+=><~!?\"'*#@&(){}[],.:;/| ", true)]
        [InlineData("\\", true)]
        [InlineData("\n", true)]
        [InlineData("кирилица", false)]
        [InlineData("^", false)]
        [InlineData("%", false)]
        public void Match_SimplifiedAscii_IsCorrect(string input, bool isMatch)
        {
            Regex.IsMatch(input, RegularExpressions.SimplifiedAscii).Should().Be(isMatch);
        }
    }
}