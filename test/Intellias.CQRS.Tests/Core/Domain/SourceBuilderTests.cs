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

        [Fact]
        public void BuildErrorSource_ArrayTypeExpression_SuccessTest()
        {
            var result = SourceBuilder.BuildErrorSource<TestSourceCommand>(c => c.SomeArray[0].SomeString);

            result.Should().Be("TestSourceCommand.SomeArray.SomeString");
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
