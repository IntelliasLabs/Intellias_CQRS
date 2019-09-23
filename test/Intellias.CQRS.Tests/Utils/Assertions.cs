using FluentAssertions.Equivalency;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Queries.Immutable;
using Intellias.CQRS.Core.Queries.Mutable;
using Intellias.CQRS.Tests.Utils.AssertionRules;

namespace Intellias.CQRS.Tests.Utils
{
    public static class Assertions
    {
        public static EquivalencyAssertionOptions<TIntegrationEvent> ForIntegrationEvent<TIntegrationEvent>(
            this EquivalencyAssertionOptions<TIntegrationEvent> options)
            where TIntegrationEvent : IntegrationEvent
            => options
                .Using(new IntegrationEventSelectionRule())
                .ComparingByMembers<TIntegrationEvent>();

        public static EquivalencyAssertionOptions<TMutableQueryModel> ForMutableQueryModel<TMutableQueryModel>(
            this EquivalencyAssertionOptions<TMutableQueryModel> options)
            where TMutableQueryModel : IMutableQueryModel
            => options
                .Using(new MutableQueryModelSelectionRule())
                .ComparingByMembers<TMutableQueryModel>();

        public static EquivalencyAssertionOptions<TImmutableQueryModel> ForImmutableQueryModel<TImmutableQueryModel>(
            this EquivalencyAssertionOptions<TImmutableQueryModel> options)
            where TImmutableQueryModel : IImmutableQueryModel
            => options
                .Using(new ImmutableQueryModelSelectionRule())
                .ComparingByMembers<TImmutableQueryModel>();
    }
}