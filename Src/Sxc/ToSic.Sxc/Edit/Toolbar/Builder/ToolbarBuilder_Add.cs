using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        /// <inheritdoc />
        [PrivateApi]
        public IToolbarBuilder ButtonAdd(params string[] rules)
            => AddInternal(rules?.Cast<object>().ToArray());   // Must re-to-array, so that it's not re-wrapped


        public IToolbarBuilder ButtonModify(
            string name,
            string noParamOrder = Eav.Parameters.Protector,
            //object target = null,
            object ui = null, 
            object parameters = null)
        {
            if (!name.HasValue()) return this;

            name = name.TrimStart((char)ToolbarRuleOperations.Modify);

            var rule = new ToolbarRuleCustom(/*target*/null, name, ObjToString(ui), ObjToString(parameters), (char)ToolbarRuleOperations.Modify);
            return AddInternal(rule);
        }

        public IToolbarBuilder ButtonRemove(params string[] names)
        {
            if (names == null || !names.Any()) return this;
            var realNames = names
                .Select(n => n.Trim().Trim((char)ToolbarRuleOperations.Remove))
                .Where(n => n.HasValue()).ToList();
            if (!realNames.Any()) return this;

            var rules = realNames.Select(n => (char)ToolbarRuleOperations.Remove + n);
            return AddInternal(rules.Cast<object>().ToArray());
        }

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
