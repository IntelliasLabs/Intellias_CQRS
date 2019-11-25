using System;
using System.Collections.Generic;
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

            result.Should().BeOfType<FailedResult>()
                .Which.Code.Should().Be(CoreErrorCodes.ValidationFailed.Code);
        }

        [Fact]
        public void Validate_TypeWithCustomNestedTypeInvalid_Fails()
        {
            var instance = new TypeWithCustomNestedType { Property2 = new NestedType() };
            var result = RecursiveValidator.Validate(instance);

            result.Should().BeOfType<FailedResult>();

            var errorDetails = ((FailedResult)result).Details.ToArray();
            errorDetails[0].Source.Should().Be($"{nameof(TypeWithCustomNestedType)}.{nameof(TypeWithCustomNestedType.Property1)}");
            errorDetails[0].CodeInfo.Code.Should().BeEquivalentTo(AnnotationErrorCodes.Required.Code);
            errorDetails[1].Source.Should().Be($"{nameof(TypeWithCustomNestedType)}.{nameof(TypeWithCustomNestedType.Property2)}.{nameof(NestedType.NestedProperty)}");
            errorDetails[1].CodeInfo.Code.Should().BeEquivalentTo(AnnotationErrorCodes.Required.Code);
        }

        [Fact]
        public void Validate_TypeWithCustomNestedTypeIsNull_Succeeds()
        {
            var instance = new TypeWithCustomNestedType { Property1 = "x" };
            var result = RecursiveValidator.Validate(instance);

            result.Should().BeOfType<SuccessfulResult>();
        }

        [Fact]
        public void Validate_TypeWithArrayOfCustomNestedTypesIsNotSetup_Fails()
        {
            var instance = new TypeWithArrayOfCustomNestedTypes();

            var result = RecursiveValidator.Validate(instance);

            result.Should().BeOfType<FailedResult>();

            var errorDetails = ((FailedResult)result).Details.ToArray();
            errorDetails[0].Source.Should().Be($"{nameof(TypeWithArrayOfCustomNestedTypes)}.{nameof(TypeWithArrayOfCustomNestedTypes.Property)}");
            errorDetails[0].CodeInfo.Code.Should().BeEquivalentTo(AnnotationErrorCodes.Required.Code);
            errorDetails[1].Source.Should().Be($"{nameof(TypeWithArrayOfCustomNestedTypes)}.{nameof(TypeWithArrayOfCustomNestedTypes.NestedTypeArray)}");
            errorDetails[1].CodeInfo.Code.Should().BeEquivalentTo(AnnotationErrorCodes.NotEmpty.Code);
        }

        [Fact]
        public void Validate_TypeWithArrayOfCustomNestedTypesWithArrayPropertyIsSetupPartially_Fails()
        {
            var instance = new TypeWithArrayOfCustomNestedTypes
            {
                NestedTypeArray = new[]
                {
                    null, // Should fail coz collection element is required.
                    new NestedType { NestedProperty = "Some value" },
                    new NestedType() // Should fail coz there is no setup required property.
                }
            };

            var result = RecursiveValidator.Validate(instance);

            result.Should().BeOfType<FailedResult>()
                .Which.Code.Should().Be(CoreErrorCodes.ValidationFailed.Code);

            var errorDetails = ((FailedResult)result).Details.ToArray();

            errorDetails.Length.Should().Be(3);
            errorDetails[0].Source.Should().Be($"{nameof(TypeWithArrayOfCustomNestedTypes)}.{nameof(TypeWithArrayOfCustomNestedTypes.Property)}");
            errorDetails[0].CodeInfo.Code.Should().BeEquivalentTo(AnnotationErrorCodes.Required.Code);
            errorDetails[1].Source.Should().Be($"{nameof(TypeWithArrayOfCustomNestedTypes)}.{nameof(TypeWithArrayOfCustomNestedTypes.NestedTypeArray)}.0");
            errorDetails[1].CodeInfo.Code.Should().BeEquivalentTo(AnnotationErrorCodes.Required.Code);
            errorDetails[2].Source.Should().Be($"{nameof(TypeWithArrayOfCustomNestedTypes)}.{nameof(TypeWithArrayOfCustomNestedTypes.NestedTypeArray)}.2.{nameof(NestedType.NestedProperty)}");
            errorDetails[2].CodeInfo.Code.Should().BeEquivalentTo(AnnotationErrorCodes.Required.Code);
        }

        [Fact]
        public void Validate_TypeWithArrayOfCustomNestedTypesWithArrayPropertyHasUkrainianLetters_Fails()
        {
            var instance = new TypeWithArrayOfCustomNestedTypes
            {
                Property = "Some property",
                NestedTypeArray = new[]
                {
                    new NestedType { NestedProperty = "Some value" },
                    new NestedType { NestedProperty = "я люблю українську мову" } // Should fail coz of regular expression verification.
                }
            };

            var result = RecursiveValidator.Validate(instance);

            result.Should().BeOfType<FailedResult>()
                .Which.Code.Should().Be(CoreErrorCodes.ValidationFailed.Code);

            var errorDetails = ((FailedResult)result).Details.ToArray();

            errorDetails.Length.Should().Be(1);
            errorDetails[0].Source.Should().Be($"{nameof(TypeWithArrayOfCustomNestedTypes)}.{nameof(TypeWithArrayOfCustomNestedTypes.NestedTypeArray)}.1.{nameof(NestedType.NestedProperty)}");
            errorDetails[0].CodeInfo.Code.Should().BeEquivalentTo(AnnotationErrorCodes.RegularExpression.Code);
        }

        [Fact]
        public void Validate_TypeWithArrayOfCustomNestedTypesWithStringsArrayIsInvalid_Fails()
        {
            var instance = new TypeWithArrayOfCustomNestedTypes
            {
                Property = "Some property",
                NestedTypeArray = new[]
                {
                    new NestedType { NestedProperty = "Some value" },
                    new NestedType
                    {
                        NestedProperty = "Some another value",
                        StringsArray = new[]
                        {
                            "123", // String is too short.
                            "some valid value",
                            "я люблю українську мову" // String contsins not English letters.
                        }
                    },
                }
            };

            var result = RecursiveValidator.Validate(instance);

            result.Should().BeOfType<FailedResult>()
                .Which.Code.Should().Be(CoreErrorCodes.ValidationFailed.Code);

            var errorDetails = ((FailedResult)result).Details.ToArray();

            errorDetails.Length.Should().Be(2);
            errorDetails[0].Source.Should().Be($"{nameof(TypeWithArrayOfCustomNestedTypes)}.{nameof(TypeWithArrayOfCustomNestedTypes.NestedTypeArray)}.1.{nameof(NestedType.StringsArray)}.0");
            errorDetails[0].CodeInfo.Code.Should().BeEquivalentTo(AnnotationErrorCodes.MinLength.Code);
            errorDetails[1].Source.Should().Be($"{nameof(TypeWithArrayOfCustomNestedTypes)}.{nameof(TypeWithArrayOfCustomNestedTypes.NestedTypeArray)}.1.{nameof(NestedType.StringsArray)}.2");
            errorDetails[1].CodeInfo.Code.Should().BeEquivalentTo(AnnotationErrorCodes.RegularExpression.Code);
        }

        [Fact]
        public void Validate_TypeWithArrayOfCustomNestedTypes_SuccessTest()
        {
            var instance = new TypeWithArrayOfCustomNestedTypes
            {
                Property = "Some property",
                NestedTypeArray = new[]
                {
                    new NestedType { NestedProperty = "Some value" },
                    new NestedType { NestedProperty = "Some another value" } // Should fail coz of regular expression verification.
                }
            };

            var result = RecursiveValidator.Validate(instance);

            result.Success.Should().BeTrue();
        }

        [Fact]
        public void Validate_TypeWithArrayOfIntegersIsEmpty_Fails()
        {
            var instance = new TypeWithArrayOfIntegers
            {
                IntegersArray = Array.Empty<int>()
            };

            var result = RecursiveValidator.Validate(instance);

            result.Should().BeOfType<FailedResult>()
                .Which.Code.Should().Be(CoreErrorCodes.ValidationFailed.Code);

            var errorDetails = ((FailedResult)result).Details.ToArray();

            errorDetails.Length.Should().Be(1);
            errorDetails[0].Source.Should().Be($"{nameof(TypeWithArrayOfIntegers)}.{nameof(TypeWithArrayOfIntegers.IntegersArray)}");
            errorDetails[0].CodeInfo.Code.Should().BeEquivalentTo(AnnotationErrorCodes.NotEmpty.Code);
        }

        [Fact]
        public void Validate_TypeWithArrayOfIntegersOverheadsRange_Fails()
        {
            var instance = new TypeWithArrayOfIntegers
            {
                IntegersArray = new[] { 11, 111 }
            };

            var result = RecursiveValidator.Validate(instance);

            result.Should().BeOfType<FailedResult>()
                .Which.Code.Should().Be(CoreErrorCodes.ValidationFailed.Code);

            var errorDetails = ((FailedResult)result).Details.ToArray();

            errorDetails.Length.Should().Be(1);
            errorDetails[0].Source.Should().Be($"{nameof(TypeWithArrayOfIntegers)}.{nameof(TypeWithArrayOfIntegers.IntegersArray)}.1");
            errorDetails[0].CodeInfo.Code.Should().BeEquivalentTo(AnnotationErrorCodes.Range.Code);
        }

        [Fact]
        public void Validate_TypeWithArrayOfIntegersAreCorrect_Success()
        {
            var instance = new TypeWithArrayOfIntegers
            {
                IntegersArray = new[] { 11, 50, 99, 100, 10 }
            };

            var result = RecursiveValidator.Validate(instance);

            result.Should().BeOfType<SuccessfulResult>();
        }

        [Fact]
        public void Validate_TypeWithListOfIntegersOverheadsRange_Fails()
        {
            var instance = new TypeWithListOfIntegers
            {
                IntegersList = new List<int> { 11, 80, 999, 77 }
            };

            var result = RecursiveValidator.Validate(instance);

            result.Should().BeOfType<FailedResult>()
                .Which.Code.Should().Be(CoreErrorCodes.ValidationFailed.Code);

            var errorDetails = ((FailedResult)result).Details.ToArray();

            errorDetails.Length.Should().Be(1);
            errorDetails[0].Source.Should().Be($"{nameof(TypeWithListOfIntegers)}.{nameof(TypeWithListOfIntegers.IntegersList)}.2");
            errorDetails[0].CodeInfo.Code.Should().BeEquivalentTo(AnnotationErrorCodes.Range.Code);
        }

        [Fact]
        public void Validate_TypeWithListOfIntegersAreCorrect_Success()
        {
            var instance = new TypeWithListOfIntegers
            {
                IntegersList = new List<int> { 11, 50, 99, 100, 10 }
            };

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

        private class TypeWithArrayOfCustomNestedTypes
        {
            [Required]
            public string Property { get; set; }

            [Required]
            [NotEmpty]
            [ValidateCollection(typeof(RequiredAttribute))]
            public NestedType[] NestedTypeArray { get; set; } = Array.Empty<NestedType>();
        }

        private class NestedType
        {
            [Required(ErrorMessage = nameof(RequiredAttribute))]
            [RegularExpression(RegularExpressions.NameIdentifier)]
            public string NestedProperty { get; set; }

            [ValidateCollection(typeof(RegularExpressionAttribute), RegularExpressions.NameIdentifier)]
            [ValidateCollection(typeof(MinLengthAttribute), 5)]
            public string[] StringsArray { get; set; }
        }

        private class TypeWithArrayOfIntegers
        {
            [Required]
            [NotEmpty]
            [ValidateCollection(typeof(RangeAttribute), 10, 100)]
            public int[] IntegersArray { get; set; }
        }

        private class TypeWithListOfIntegers
        {
            [Required]
            [NotEmpty]
            [ValidateCollection(typeof(RangeAttribute), 10, 100)]
            public List<int> IntegersList { get; set; }
        }
    }
}