using System.Linq;
using ToSic.Eav.Documentation;
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
            object ui = null, 
            object parameters = null)
        {
            if (!name.HasValue()) return this;

            name = name.TrimStart((char)ToolbarRuleOperations.BtnModify);

            var rule = new ToolbarRuleCustom(name, ui: ObjToString(ui), parameters: ObjToString(parameters), (char)ToolbarRuleOperations.BtnModify);
            return AddInternal(rule);
        }

        public IToolbarBuilder ButtonRemove(params string[] names)
        {
            if (names == null || !names.Any()) return this;
            var realNames = names
                .Select(n => n.Trim().Trim((char)ToolbarRuleOperations.BtnRemove))
                .Where(n => n.HasValue()).ToList();
            if (!realNames.Any()) return this;

            var rules = realNames.Select(n => (char)ToolbarRuleOperations.BtnRemove + n);
            return AddInternal(rules.Cast<object>().ToArray());
        }
        

    }
}
