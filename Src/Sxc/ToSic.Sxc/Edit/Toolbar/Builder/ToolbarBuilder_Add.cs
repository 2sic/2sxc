using System.Linq;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        /// <inheritdoc />
        public IToolbarBuilder AddInternal(params object[] newRules)
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
