using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Intellias.CQRS.Core.DataAnnotations;
using Intellias.CQRS.Core.DataAnnotations.Validators;
using Intellias.CQRS.Core.Results.Errors;
using Xunit;

namespace Intellias.CQRS.Tests.Core.DataAnnotations
{
    public class AnnotationErrorCodesTests
    {
        [Fact]
        public void ErrorCodes_Always_PresentForAllAnnotations()
        {
            var validationAttributes = GetValidationAttributes(typeof(Validator))
                .Union(GetValidationAttributes(typeof(CoreValidator)))
                .Select(AnnotationErrorCodes.GetErrorCodeFromAttribute)
                .OrderBy(c => c)
                .ToArray();

            var errorCodes = typeof(AnnotationErrorCodes).GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(m => m.PropertyType == typeof(ErrorCodeInfo))
                .Select(m => (ErrorCodeInfo)m.GetValue(null))
                .Select(e => e.Code)
                .OrderBy(c => c)
                .ToArray();

            validationAttributes.Should().BeEquivalentTo(errorCodes);
        }

        private static IEnumerable<Type> GetValidationAttributes(Type assemblyDefinedType)
        {
            return Assembly.GetAssembly(assemblyDefinedType)
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(ValidationAttribute).IsAssignableFrom(t));
        }
    }
}