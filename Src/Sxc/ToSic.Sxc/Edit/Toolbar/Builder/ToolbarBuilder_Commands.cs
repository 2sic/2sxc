using ToSic.Eav;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Url;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleForEntity;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        public IToolbarBuilder Add(object target,
            string noParamOrder = "Rule: All params must be named (https://r.2sxc.org/named-params)",
            string contentType = null,
            object ui = null,
            object parameters = null)
        {
            // Special case: Add could be called to "add a button"
            // There is an edge case in the Events app where this was published
            // Must decide if we should keep this or not
            if (target is string strTarget && strTarget.HasValue() && strTarget.Length > 3)
                return AddInternal(strTarget);

            var addCommand =
                new ToolbarRuleForEntity(target, "add", contentType: contentType, ui: ObjToString(ui), parameters: ObjToString(parameters))
                {
                    ParamEntityIdUsed = false,
                    ParamContentTypeUsed = false
                };

            return AddInternal(addCommand);
        }

        public IToolbarBuilder Edit(
            object target = null, 
            string noParamOrder = Parameters.Protector, 
            int? entityId = null,
            object ui = null, 
            object parameters = null,
            object prefill = null)
        {
            var editCommand =
                new ToolbarRuleForEntity(target, "edit", entityId: entityId, ui: new ObjectToUrl().SerializeWithChild(ui, prefill, PrefixPrefill), parameters: ObjToString(parameters))
                {
                    ParamEntityIdUsed = true,
                    ParamContentTypeUsed = false
                };
            return AddInternal(editCommand);
        }


        /// <inheritdoc />
        public IToolbarBuilder Metadata(object target,
            string contentTypes = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string context = null)
        {
            var finalTypes = GetMetadataTypeNames(target, contentTypes);
            var realContext = GetContext(target, context);
            var builder = this as IToolbarBuilder;
            foreach (var type in finalTypes)
                builder = builder.AddInternal(new ToolbarRuleMetadata(target, type, ObjToString(ui), ObjToString(parameters), context: realContext, helper: _deps.ToolbarButtonHelper.Ready));

            return builder;
        }

        /// <inheritdoc />
        public IToolbarBuilder Copy(object target,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string context = null) => AddInternal(new ToolbarRuleCopy(target, ObjToString(ui), ObjToString(parameters), GetContext(target, context), _deps.ToolbarButtonHelper.Ready));


        [PrivateApi("WIP 13.11")]
        public IToolbarBuilder Image(
            object target,
            string noParamOrder = Eav.Parameters.Protector,
            string ui = null,
            string parameters = null
        ) => AddInternal(new ToolbarRuleImage(target, ui, parameters, context: GetContext(target, null), helper: _deps.ToolbarButtonHelper.Ready));
    }
}
