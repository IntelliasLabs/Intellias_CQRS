using FluentAssertions.Equivalency;
using Intellias.CQRS.Core.Queries.Mutable;
using Intellias.CQRS.Tests.Utils.AssertionRules;

namespace Intellias.CQRS.Tests.Utils
{
    public static class Assertions
    {
        public static EquivalencyAssertionOptions<TMutableQueryModel> ForQueryModel<TMutableQueryModel>(
            this EquivalencyAssertionOptions<TMutableQueryModel> options)
            where TMutableQueryModel : IMutableQueryModel
            => options
                .Using(new MutableQueryModelSelectionRule())
                .ComparingByMembers<TMutableQueryModel>();
    }
}