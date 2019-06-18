using System;
using FluentAssertions;
using Intellias.CQRS.Core.Exceptions;
using Xunit;

namespace Intellias.CQRS.Tests.Core.Exceptions
{
    public class EnsureTests
    {
        [Fact]
        public void ThatWithInvalidConditionRuleShoudThrowBusinessRuleValidationException()
        {
            var validationMessage = "invalid rule";

            Action act = () => Ensure.That(() => false, validationMessage);

            act.Should().Throw<BusinessRuleValidationException>()
                .WithMessage(validationMessage);
        }

        [Fact]
        public void ThatWithInvalidRuleShouldThrowBusinessRuleValidationException()
        {
            var validationMessage = "invalid rule";

            Action act = () => Ensure.That(false, validationMessage);

            act.Should().Throw<BusinessRuleValidationException>()
                .WithMessage(validationMessage);
        }

        [Fact]
        public void ThatWithValidRuleSohudNotThrowException()
        {
            var validationMessage = "invalid rule";

            Action act = () => Ensure.That(true, validationMessage);

            act.Should().NotThrow<Exception>();
        }

        [Fact]
        public void ThatWithValidConditionRuleSohudNotThrowException()
        {
            var validationMessage = "invalid rule";

            Action act = () => Ensure.That(() => true, validationMessage);

            act.Should().NotThrow<Exception>();
        }
    }
}
