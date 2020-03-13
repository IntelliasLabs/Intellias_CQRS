using FluentAssertions.Equivalency;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Queries.Immutable;
using Intellias.CQRS.Core.Queries.Mutable;

namespace Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules
{
    /// <summary>
    /// Assertion rules for solution.
    /// </summary>
    public static class CoreAssertions
    {
        /// <summary>
        /// Assertion rule for <see cref="TMessage"/>.
        /// </summary>
        /// <param name="options">Assertion options.</param>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <returns>Configured assertion.</returns>
        public static EquivalencyAssertionOptions<TMessage> ForMessage<TMessage>(
            this EquivalencyAssertionOptions<TMessage> options)
            where TMessage : IMessage
            => options
                .Excluding(m => m.Id)
                .Excluding(m => m.Created)
                .ExcludingMissingMembers()
                .ComparingByMembers<IMessage>();

        /// <summary>
        /// Assertion rule for <see cref="TIntegrationEvent"/>.
        /// </summary>
        /// <param name="options">Assertion options.</param>
        /// <typeparam name="TIntegrationEvent">Type of the integration event.</typeparam>
        /// <returns>Configured assertion.</returns>
        public static EquivalencyAssertionOptions<TIntegrationEvent> ForIntegrationEvent<TIntegrationEvent>(
            this EquivalencyAssertionOptions<TIntegrationEvent> options)
            where TIntegrationEvent : IntegrationEvent
            => options
                .Using(new IntegrationEventSelectionRule())
                .ComparingByMembers<TIntegrationEvent>();

        /// <summary>
        /// Assertion rule for <see cref="TSignal"/>.
        /// </summary>
        /// <param name="options">Assertion options.</param>
        /// <typeparam name="TSignal">Type of the signal.</typeparam>
        /// <returns>Configured assertion.</returns>
        public static EquivalencyAssertionOptions<TSignal> ForSignal<TSignal>(
            this EquivalencyAssertionOptions<TSignal> options)
            where TSignal : IMessage
            => options
                .Using(new SignalSelectionRule())
                .ComparingByMembers<TSignal>();

        /// <summary>
        /// Assertion rule for <see cref="TMutableQueryModel"/>.
        /// </summary>
        /// <param name="options">Assertion options.</param>
        /// <typeparam name="TMutableQueryModel">Type of the query model.</typeparam>
        /// <returns>Configured assertion.</returns>
        public static EquivalencyAssertionOptions<TMutableQueryModel> ForMutableQueryModel<TMutableQueryModel>(
            this EquivalencyAssertionOptions<TMutableQueryModel> options)
            where TMutableQueryModel : IMutableQueryModel
            => options
                .Using(new MutableQueryModelSelectionRule())
                .ComparingByMembers<TMutableQueryModel>();

        /// <summary>
        /// Assertion rule for <see cref="TImmutableQueryModel"/>.
        /// </summary>
        /// <param name="options">Assertion options.</param>
        /// <typeparam name="TImmutableQueryModel">Type of the query model.</typeparam>
        /// <returns>Configured assertion.</returns>
        public static EquivalencyAssertionOptions<TImmutableQueryModel> ForImmutableQueryModel<TImmutableQueryModel>(
            this EquivalencyAssertionOptions<TImmutableQueryModel> options)
            where TImmutableQueryModel : IImmutableQueryModel
            => options
                .Using(new ImmutableQueryModelSelectionRule())
                .ComparingByMembers<TImmutableQueryModel>();
    }
}