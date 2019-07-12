using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Core.Commands
{
    /// <inheritdoc cref="ICommand" />
    public abstract class Command : AbstractMessage, ICommand, IValidatableObject
    {
        /// <inheritdoc />
        public int ExpectedVersion { get; set; }

        /// <summary>
        /// Converts common command data to event
        /// </summary>
        /// <typeparam name="TEvent">event without specific properties</typeparam>
        /// <returns></returns>
        public TEvent ToEvent<TEvent>()
            where TEvent : Event, new()
        {
            var e = this.ToType<TEvent>();
            e.SourceId = Id;
            e.Version = ExpectedVersion;

            return e;
        }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();

            if (!Metadata.ContainsKey(MetadataKey.Roles))
            {
                validationResults.Add(new ValidationResult($"'{nameof(MetadataKey.Roles)}' should be set in the command '{Id}", new[] { nameof(MetadataKey.Roles) }));
            }

            if (!Metadata.ContainsKey(MetadataKey.UserId))
            {
                validationResults.Add(new ValidationResult($"'{nameof(MetadataKey.UserId)}' should be set in the command '{Id}", new[] { nameof(MetadataKey.UserId) }));
            }
            else if (!Guid.TryParse(Metadata[MetadataKey.UserId], out _))
            {
                validationResults.Add(new ValidationResult($"'{nameof(MetadataKey.UserId)}' can't be parsed to guid in the command '{Id}", new[] { nameof(MetadataKey.UserId) }));
            }

            return validationResults;
        }

        /// <inheritdoc />
        public IExecutionResult Validate()
        {
            var validationContext = new ValidationContext(this);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(this, validationContext, validationResults, true);
            if(isValid)
            {
                return new SuccessfulResult();
            }
            else
            {
                var result = new FailedResult(ErrorCodes.ValidationFailed, GetType().Name, "Command Validation Failed, please, look at inner errors");
                foreach(var validationResult in validationResults)
                {
                    var field = validationResult.MemberNames.FirstOrDefault() ?? string.Empty;
                    var error = new ExecutionError(field, validationResult.ErrorMessage);
                    result.AddError(error);
                }
                return result;
            }
        }
    }
}
