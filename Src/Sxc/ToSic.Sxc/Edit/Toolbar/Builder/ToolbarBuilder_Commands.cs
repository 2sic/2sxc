using ToSic.Eav;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Web.Url;
using static ToSic.Sxc.Edit.Toolbar.EntityEditInfo;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleForEntity;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOperations;


namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        public IToolbarBuilder Delete(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null)
        {
            Parameters.Protect(noParamOrder, "See docs");

            // Set default operation based on what toolbar is used
            var isDefToolbar = FindRule<ToolbarRuleToolbar>()?.IsDefault ?? false;
            var defOp = isDefToolbar ? BtnModify : BtnAdd;

            var editCommand = new ToolbarRuleForEntity("delete", target,
                operation: ToolbarRuleOps.Pick(operation, defOp),
                ui: new ObjectToUrl().SerializeWithChild(ui, "show=true", ""),
                parameters: ObjToString(parameters),
                propsToSerialize: new[] { KeyTitle, KeyEntityId, KeyEntityGuid });
            return AddInternal(editCommand);
        }

        public IToolbarBuilder Edit(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            object prefill = null)
        {
            Parameters.Protect(noParamOrder, "See docs");
            var editCommand = new ToolbarRuleForEntity("edit", target, 
                ui: new ObjectToUrl().SerializeWithChild(ui, prefill, PrefixPrefill),
                parameters: ObjToString(parameters), 
                propsToNotSerialize: new []{ KeyEntityGuid, KeyTitle, KeyPublished});
            return AddInternal(editCommand);
        }

        public IToolbarBuilder Publish(
            object target = null,
            string noParamOrder = Parameters.Protector,
            bool? show = null,
            object ui = null,
            object parameters = null)
        {
            Parameters.Protect(noParamOrder, "See docs");
            var editCommand = new ToolbarRuleForEntity("publish", target,
                operation: OperationShow(show),
                ui: ObjToString(ui), parameters: ObjToString(parameters),
                propsToSerialize: new[] { KeyEntityId, KeyPublished, KeyIndex, KeyUseModule });
            return AddInternal(editCommand);
        }


        /// <inheritdoc />
        public IToolbarBuilder Metadata(
            object target,
            string contentTypes = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string context = null)
        {
            Parameters.Protect(noParamOrder, "See docs");
            var finalTypes = GetMetadataTypeNames(target, contentTypes);
            var realContext = GetContext(target, context);
            var builder = this as IToolbarBuilder;
            foreach (var type in finalTypes)
                builder = builder.AddInternal(new ToolbarRuleMetadata(target, type, ObjToString(ui), ObjToString(parameters), context: realContext, helper: _deps.ToolbarButtonHelper.Ready));

            return builder;
        }

        /// <inheritdoc />
        public IToolbarBuilder Copy(
            object target = null,
            string noParamOrder = Parameters.Protector,
            string contentType = null,
            object ui = null,
            object parameters = null,
            string context = null)
        {
            Parameters.Protect(noParamOrder, "See docs");

            return AddInternal(new ToolbarRuleCopy(target, contentType, ObjToString(ui), ObjToString(parameters),
                GetContext(target, context), _deps.ToolbarButtonHelper.Ready));
        }




        public IToolbarBuilder Data(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object filter = null,
            object ui = null,
            object parameters = null
        )
        {
            Parameters.Protect(noParamOrder, "See docs");
            var editCommand = new ToolbarRuleForEntity("data", target, 
                ui: ObjToString(ui),
                parameters: new ObjectToUrl().SerializeWithChild(parameters, filter, PrefixFilters),
                contentType: target as string,
                propsToSerialize: new[] { KeyContentType });
            return AddInternal(editCommand);
        }



        [PrivateApi("WIP 13.11")]
        public IToolbarBuilder Image(
            object target,
            string noParamOrder = Parameters.Protector,
            string ui = null,
            string parameters = null
        )
        {
            Parameters.Protect(noParamOrder, "See docs");

            return AddInternal(new ToolbarRuleImage(target, ui, parameters, context: GetContext(target, null),
                helper: _deps.ToolbarButtonHelper.Ready));
        }
    }
}
