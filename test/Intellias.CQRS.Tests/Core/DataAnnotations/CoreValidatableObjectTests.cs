using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using Intellias.CQRS.Core.DataAnnotations;
using Intellias.CQRS.Core.DataAnnotations.Validators;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;
using Xunit;

namespace Intellias.CQRS.Tests.Core.DataAnnotations
{
    public class CoreValidatableObjectTests
    {
        [Fact]
        public void Validate_NoErrors_ValidationSucceeds()
        {
            var validatableObject = new FakeValidatableObject(Array.Empty<ExecutionError>());

            RecursiveValidator.Validate(validatableObject).Success.Should().BeTrue();
        }

        [Fact]
        public void Validate_HasErrors_ValidationFails()
        {
            var expectedError = new ExecutionError(CoreErrorCodes.ValueIsRequired, nameof(Validate_HasErrors_ValidationFails));
            var validatableObject = new FakeValidatableObject(new[] { expectedError });

            var result = (FailedResult)RecursiveValidator.Validate(validatableObject);
            var error = result.Details.Single();

            error.Code.Should().Be(expectedError.Code);
            error.Source.Should().Be($"{nameof(FakeValidatableObject)}.{expectedError.Source}");
        }

        private class FakeValidatableObject : ICoreValidatableObject
        {
            private readonly ExecutionError[] errors;

            public FakeValidatableObject(ExecutionError[] errors)
            {
                this.errors = errors;
            }

            public IEnumerable<ExecutionError> Validate(ValidationContext validationContext)
            {
                return errors;
            }
        }
    }
}