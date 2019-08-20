using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Equivalency;
using Intellias.CQRS.Core.Queries.Mutable;

namespace Intellias.CQRS.Tests.Utils.AssertionRules
{
    public class MutableQueryModelSelectionRule : IMemberSelectionRule
    {
        public bool IncludesMembers => false;

        public IEnumerable<SelectedMemberInfo> SelectMembers(
            IEnumerable<SelectedMemberInfo> selectedMembers,
            IMemberInfo context,
            IEquivalencyAssertionOptions config)
        {
            return selectedMembers.Where(m => m.Name != nameof(IMutableQueryModel.ETag) && m.Name != nameof(IMutableQueryModel.Timestamp));
        }
    }
}