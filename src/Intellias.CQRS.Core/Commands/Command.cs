using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Intellias.CQRS.Core.DataAnnotations;
using Intellias.CQRS.Core.DataAnnotations.Validators;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;
using Intellias.CQRS.Core.Security;

namespace Intellias.CQRS.Core.Commands
{
    /// <inheritdoc cref="ICommand" />
    public abstract class Command : AbstractMessage, ICommand, ICoreValidatableObject
    {
        /// <inheritdoc />
        public int ExpectedVersion { get; set; }

        /// <inheritdoc />
        public Principal Principal { get; set; } = new Principal();

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
        public IEnumerable<ExecutionError> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ExecutionError>();

            if (!Metadata.ContainsKey(MetadataKey.Roles))
            {
                validationResults.Add(new ExecutionError(
                    CoreErrorCodes.NotFound,
                    $"{nameof(Metadata)}.{nameof(MetadataKey.Roles)}",
                    $"'{nameof(MetadataKey.Roles)}' should be set in the command '{Id}."));
            }

            if (!Metadata.ContainsKey(MetadataKey.UserId))
            {
                validationResults.Add(new ExecutionError(
                    CoreErrorCodes.NotFound,
                    $"{nameof(Metadata)}.{nameof(MetadataKey.UserId)}",
                    $"'{nameof(MetadataKey.UserId)}' should be set in the command '{Id}."));
            }
            else if (!Guid.TryParse(Metadata[MetadataKey.UserId], out _))
            {
                validationResults.Add(new ExecutionError(
                    CoreErrorCodes.InvalidFormat,
                    $"{nameof(Metadata)}.{nameof(MetadataKey.UserId)}",
                    $"'{nameof(MetadataKey.UserId)}' value '{Metadata[MetadataKey.UserId]}' can't be parsed to GUID in the command '{Id}."));
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
