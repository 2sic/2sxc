using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        /// <inheritdoc />
        public IToolbarBuilder Metadata(object target,
            string contentTypes = null,
            string noParamOrder = "Rule: All params must be named (https://r.2sxc.org/named-params)",
            object ui = null,
            object parameters = null,
            string context = null)
        {
            var finalTypes = GetMetadataTypeNames(target, contentTypes);
            var realContext = GetContext(target, context);
            var builder = this as IToolbarBuilder;
            foreach (var type in finalTypes)
                builder = builder.Add(new ToolbarRuleMetadata(target, type, ObjToString(ui), ObjToString(parameters), context: realContext, helper: _deps.ToolbarButtonHelper.Ready));

            return builder;
        }

        /// <inheritdoc />
        public IToolbarBuilder Copy(object target,
            string noParamOrder = "Rule: All params must be named (https://r.2sxc.org/named-params)",
            object ui = null,
            object parameters = null,
            string context = null) => Add(new ToolbarRuleCopy(target, ObjToString(ui), ObjToString(parameters), GetContext(target, context), _deps.ToolbarButtonHelper.Ready));


        [PrivateApi("WIP 13.11")]
        public IToolbarBuilder Image(
            object target,
            string noParamOrder = Eav.Parameters.Protector,
            string ui = null,
            string parameters = null
        ) => Add(new ToolbarRuleImage(target, ui, parameters, context: GetContext(target, null), helper: _deps.ToolbarButtonHelper.Ready));
    }
}
