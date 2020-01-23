using System.Text.RegularExpressions;
using FluentAssertions;
using Intellias.CQRS.Core.DataAnnotations;
using Xunit;

namespace Intellias.CQRS.Tests.Core.DataAnnotations
{
    public class RegularExpressionsTests
    {
        [Theory]
        [InlineData("C#", true)]
        [InlineData("ASP .NET Core", true)]
        [InlineData("C++", true)]
        [InlineData("abcezAABDSCXZ0568969_-+\"'*#@&(),.:;/ ", true)]
        [InlineData("\n", true)]
        [InlineData("\\", false)]
        [InlineData("кирилица", false)]
        [InlineData("^", false)]
        [InlineData("%", false)]
        public void Match_NameIdentifier_IsCorrect(string input, bool isMatch)
        {
            Regex.IsMatch(input, RegularExpressions.NameIdentifier).Should().Be(isMatch);
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

        [Theory]
        [InlineData("C#", true)]
        [InlineData("ASP .NET Core", true)]
        [InlineData("C++", true)]
        [InlineData("abcezAABDSCXZ0-9_-+=><~!?\"'*#@&(){}[],.:;/| ", true)]
        [InlineData("`code`", true)]
        [InlineData("10%", true)]
        [InlineData("\\", true)]
        [InlineData("\n", true)]
        [InlineData("\r", true)]
        [InlineData("\t", true)]
        [InlineData(
            @"My feedback:
            1. One.
            * Two.    ", true)]
        [InlineData("кирилица", false)]
        [InlineData("^", false)]
        public void Match_Markdown_IsCorrect(string input, bool isMatch)
        {
            Regex.IsMatch(input, RegularExpressions.Markdown).Should().Be(isMatch);
        }
    }
}