using System;
using FluentAssertions;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Results.Errors;
using Xunit;

namespace Intellias.CQRS.Tests.Core.Domain
{
    public class SourceBuilderTests
    {
        [Fact]
        public void BuildErrorSource_SimpleObjectTypeExpression_SuccessTest()
        {
            var result = SourceBuilder.BuildErrorSource<TestSourceCommand>(c => c.SomeObject);

            result.Should().Be("TestSourceCommand.SomeObject");
        }

        [Fact]
        public void BuildErrorSource_SimpleIntTypeExpression_SuccessTest()
        {
            var result = SourceBuilder.BuildErrorSource<TestSourceCommand>(c => c.SomeInt);

            result.Should().Be("TestSourceCommand.SomeInt");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(999)]
        public void BuildErrorSource_ArrayTypeExpressionWithIndexParameter_SuccessTest(int index)
        {
            var result = SourceBuilder.BuildErrorSource<TestSourceCommand>(c => c.SomeArray[index].SomeString);

            result.Should().Be($"TestSourceCommand.SomeArray.{index}.SomeString");
        }

        [Fact]
        public void BuildErrorSource_ArrayTypeExpressionWithConstantIndexParameter_SuccessTest()
        {
            var result = SourceBuilder.BuildErrorSource<TestSourceCommand>(c => c.SomeArray[3].SomeString);

            result.Should().Be($"TestSourceCommand.SomeArray.3.SomeString");
        }

        private class TestSourceCommand : Command
        {
            public SomeObject SomeObject { get; set; } = new SomeObject();

            public int SomeInt { get; set; }

            public SomeObject[] SomeArray { get; set; } = Array.Empty<SomeObject>();
        }

        private class SomeObject
        {
            public string SomeString { get; set; }
        }
    }
}
