using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        /// <inheritdoc />
        [PrivateApi]
        public IToolbarBuilder Add(params string[] rules)
            => InnerAdd(rules?.Cast<object>().ToArray());   // Must re-to-array, so that it's not re-wrapped

        /// <inheritdoc />
        public IToolbarBuilder Add(params object[] rules)
            => InnerAdd(rules?.ToArray());                  // Must re-to-array, so that it's not re-wrapped

        private IToolbarBuilder InnerAdd(params object[] newRules)
        {
            // Create clone before starting to log so it's in there too
            var clone = new ToolbarBuilder(this);

            var callLog = Log.Fn<IToolbarBuilder>();
            //clone.Rules.AddRange(Rules);
            if (newRules == null || !newRules.Any())
                return callLog.Return(clone, "no new rules");

            foreach (var rule in newRules)
            {
                if (rule is ToolbarRuleBase realRule)
                    clone.Rules.Add(realRule);
                else if (rule is string stringRule)
                    clone.Rules.Add(new ToolbarRuleGeneric(stringRule));
            }
            return callLog.Return(clone);
        }

    }
}
