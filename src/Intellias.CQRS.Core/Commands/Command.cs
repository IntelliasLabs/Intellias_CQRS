using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Intellias.CQRS.Core.DataAnnotations;
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
        /// Converts common command data to event.
        /// </summary>
        /// <typeparam name="TEvent">event without specific properties.</typeparam>
        /// <returns>Event.</returns>
        public TEvent ToEvent<TEvent>()
            where TEvent : Event, new()
        {
            var e = this.ToType<TEvent>();
            e.SourceId = Id;
            e.Version = ExpectedVersion;

            return e;
        }

        /// <summary>
        /// Validate command.
        /// </summary>
        /// <param name="validationContext">Validation COntext.</param>
        /// <returns>Collection of Validation Results.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();

            if (!Metadata.ContainsKey(MetadataKey.Roles))
            {
                validationResults.Add(new ValidationResult($"'{nameof(MetadataKey.Roles)}' should be set in the command '{Id}.", new[] { nameof(MetadataKey.Roles) }));
            }

            if (!Metadata.ContainsKey(MetadataKey.UserId))
            {
                validationResults.Add(new ValidationResult($"'{nameof(MetadataKey.UserId)}' should be set in the command '{Id}.", new[] { nameof(MetadataKey.UserId) }));
            }
            else if (!Guid.TryParse(Metadata[MetadataKey.UserId], out _))
            {
                validationResults.Add(new ValidationResult($"'{nameof(MetadataKey.UserId)}' can't be parsed to guid in the command '{Id}.", new[] { nameof(MetadataKey.UserId) }));
            }

            return validationResults;
        }

        /// <inheritdoc />
        public IExecutionResult Validate()
        {
            return RecursiveValidator.Validate(this);
        }
    }
}
