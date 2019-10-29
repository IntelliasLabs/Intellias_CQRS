using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using Intellias.CQRS.Core.DataAnnotations;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;
using Xunit;

namespace Intellias.CQRS.Tests.Core.DataAnnotations
{
    public class RecursiveValidatorTests
    {
        [Fact]
        public void Validate_InstanceIsNull_Succeeds()
        {
            var result = RecursiveValidator.Validate(null);

            result.Should().BeOfType<SuccessfulResult>();
        }

        [Fact]
        public void Validate_TypeWithValidPrimitiveType_Fails()
        {
            var instance = new TypeWithPrimitiveTypes { Property = "x" };
            var result = RecursiveValidator.Validate(instance);

            result.Should().BeOfType<SuccessfulResult>();
        }

        [Fact]
        public void Validate_TypeWithInvalidPrimitiveType_Fails()
        {
            var instance = new TypeWithPrimitiveTypes();
            var result = RecursiveValidator.Validate(instance);

            result.Should().BeOfType<FailedResult>();
        }

        [Fact]
        public void Validate_TypeWithCustomNestedTypeInvalid_Fails()
        {
            var instance = new TypeWithCustomNestedType { Property2 = new NestedType() };
            var result = RecursiveValidator.Validate(instance);

            result.Should().BeOfType<FailedResult>();

            var errorDetails = ((FailedResult)result).Details.ToArray();
            errorDetails[0].Source.Should().Be($"{nameof(TypeWithCustomNestedType)}.{nameof(TypeWithCustomNestedType.Property1)}");
            errorDetails[0].CodeInfo.Should().BeEquivalentTo(CoreErrorCodes.ValidationFailed);
            errorDetails[1].Source.Should().Be($"{nameof(TypeWithCustomNestedType)}.{nameof(TypeWithCustomNestedType.Property2)}.{nameof(NestedType.NestedProperty)}");
            errorDetails[1].CodeInfo.Should().BeEquivalentTo(CoreErrorCodes.ValidationFailed);
        }

        [Fact]
        public void Validate_TypeWithCustomNestedTypeIsNull_Succeeds()
        {
            var instance = new TypeWithCustomNestedType { Property1 = "x" };
            var result = RecursiveValidator.Validate(instance);

            result.Should().BeOfType<SuccessfulResult>();
        }

        private class TypeWithPrimitiveTypes
        {
            [Required]
            public string Property { get; set; }
        }

        private class TypeWithCustomNestedType
        {
            [Required]
            public string Property1 { get; set; }

            public NestedType Property2 { get; set; }
        }

        private class NestedType
        {
            [Required]
            public string NestedProperty { get; set; }
        }
    }
}