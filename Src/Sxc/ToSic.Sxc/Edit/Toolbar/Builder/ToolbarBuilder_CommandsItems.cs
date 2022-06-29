using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Documentation;
using static ToSic.Sxc.Edit.Toolbar.EntityEditInfo;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleForEntity;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOps;


namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {

        private (char Operation, string Ui, string Parameters) PrecleanParams(
            string operation, 
            ToolbarRuleOps defOp, 
            object ui, 
            object uiMerge, 
            string uiMergePrefix, 
            object parameters, 
            object prefill,
            object filter = null)
        {
            var o2u = O2U;
            return (
                ToolbarRuleOperation.Pick(operation, defOp),
                Ui: o2u.SerializeWithChild(ui, uiMerge, uiMergePrefix),
                Parameters: o2u.SerializeWithChild(
                    o2u.SerializeWithChild(parameters, prefill, PrefixPrefill),
                    filter, PrefixFilters)
            );

        }

        private (ToolbarRuleForEntity Rule, IToolbarBuilder Builder) EntityRule(
            string verb, 
            object target,
            (char Operation, string Ui, string Parameters) pars,
            string [] propsSkip = null,
            string[] propsKeep = null,
            string contentType = null
            )
        {
            var command = new ToolbarRuleForEntity(verb, target, pars.Operation,
                ui: pars.Ui, parameters: pars.Parameters,
                contentType: contentType,
                propsKeep: propsKeep, propsSkip: propsSkip,
                decoHelper: _deps.ToolbarButtonHelper.Ready);
            var builder = AddInternal(command);
            return (command, builder);
        }

        public IToolbarBuilder Delete(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null)
        {
            Eav.Parameters.Protect(noParamOrder, "See docs");

            // Set default operation based on what toolbar is used
            var isDefToolbar = FindRule<ToolbarRuleToolbar>()?.IsDefault ?? false;
            var defOp = isDefToolbar ? OprModify : OprAdd;

            var pars = PrecleanParams(operation, defOp, ui, "show=true", "", parameters, null);

            return EntityRule("delete", target, pars, 
                propsKeep: new[] { KeyTitle, KeyEntityId, KeyEntityGuid }).Builder;

            //var editCommand = new ToolbarRuleForEntity("delete", target,
            //    operation: pars.Operation,// ToolbarRuleOps.Pick(operation, defOp),
            //    ui: pars.Ui,// new ObjectToUrl().SerializeWithChild(ui, "show=true", ""),
            //    parameters: pars.Parameters,// ObjToString(parameters),
            //    propsToSerialize: new[] { KeyTitle, KeyEntityId, KeyEntityGuid });
            //return AddInternal(editCommand);
        }

        public IToolbarBuilder Edit(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            object prefill = null,
            string operation = null)
        {
            Eav.Parameters.Protect(noParamOrder, "See docs");
            var pars = PrecleanParams(operation, OprAdd, ui, null, null, parameters, prefill);

            return EntityRule("edit", target, pars, propsSkip: new[] { KeyEntityGuid, KeyTitle, KeyPublished }).Builder;

            //var editCommand = new ToolbarRuleForEntity("edit", target, 
            //    operation: pars.Operation,// ToolbarRuleOps.Pick(operation, BtnAdd),
            //    ui: pars.Ui,// ObjToString(ui),
            //    parameters: pars.Parameters,// new ObjectToUrl().SerializeWithChild(parameters, prefill, PrefixPrefill),
            //    propsSkip: new []{ KeyEntityGuid, KeyTitle, KeyPublished});
            //return AddInternal(editCommand);
        }

        public IToolbarBuilder New(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            object prefill = null,
            string operation = null)
        {
            Eav.Parameters.Protect(noParamOrder, "See docs");
            var pars = PrecleanParams(operation, OprAdd, ui, null, null, parameters, prefill);

            return EntityRule("new", target, pars,
                propsSkip: new[] { KeyEntityGuid, KeyEntityId, KeyTitle, KeyPublished },
                contentType: target as string).Builder;

            //var editCommand = new ToolbarRuleForEntity("new", target,
            //    operation: pars.Operation,// ToolbarRuleOps.Pick(operation, BtnAdd),
            //    ui: pars.Ui,// ObjToString(ui),
            //    parameters: pars.Parameters,// new ObjectToUrl().SerializeWithChild(parameters, prefill, PrefixPrefill),
            //    contentType: target as string,
            //    propsSkip: new []{ KeyEntityGuid, KeyEntityId, KeyTitle, KeyPublished })
            //{
            //    //EditInfo =
            //    //{
            //    //    // Must set entityId to 0 ?
            //    //    entityId = 0
            //    //}
            //};
            //// TODO: TEST ENTITY ID

            //return AddInternal(editCommand);
        }

        public IToolbarBuilder Publish(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null)
        {
            Eav.Parameters.Protect(noParamOrder, "See docs");
            var pars = PrecleanParams(operation, OprAdd, ui, null, null, parameters, null);

            return EntityRule("publish", target, pars,
                propsKeep: new[] { KeyEntityId, KeyPublished, KeyIndex, KeyUseModule }).Builder;
            //var editCommand = new ToolbarRuleForEntity("publish", target,
            //    operation: pars.Operation,// ToolbarRuleOps.Pick(operation, BtnAddAuto),
            //    ui: pars.Ui /*ObjToString(ui)*/, parameters: pars.Parameters, // ObjToString(parameters),
            //    propsKeep: new[] { KeyEntityId, KeyPublished, KeyIndex, KeyUseModule });
            //return AddInternal(editCommand);
        }


        /// <inheritdoc />
        public IToolbarBuilder Metadata(
            object target,
            string contentTypes = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            object prefill = null,
            string operation = null,
            string context = null)
        // TODO: PREFILL
        {
            Eav.Parameters.Protect(noParamOrder, "See docs");
            var finalTypes = GetMetadataTypeNames(target, contentTypes);
            var realContext = GetContext(target, context);
            var builder = this as IToolbarBuilder;

            var pars = PrecleanParams(operation, OprAdd, ui, null, null, parameters, prefill);


            var mdsToAdd = finalTypes
                .Select(type => new ToolbarRuleMetadata(target, type,
                    operation: pars.Operation, // ToolbarRuleOps.Pick(operation, BtnAdd),
                    ui: pars.Ui, //ObjToString(ui),
                    parameters: pars.Parameters, //ObjToString(parameters),
                    context: realContext,
                    decoHelper: _deps.ToolbarButtonHelper.Ready));

            return builder.AddInternal(mdsToAdd.Cast<object>().ToArray());

            //foreach (var type in finalTypes)
            //    builder = builder.AddInternal(new ToolbarRuleMetadata(target, type,
            //        operation: ToolbarRuleOps.Pick(operation, BtnAdd),
            //        ObjToString(ui),
            //        ObjToString(parameters),
            //        context: realContext,
            //        helper: _deps.ToolbarButtonHelper.Ready));

            //return builder;
        }

        /// <inheritdoc />
        public IToolbarBuilder Copy(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            string contentType = null,
            object ui = null,
            object parameters = null,
            object prefill = null,
            string operation = null,
            string context = null)
        {
            Eav.Parameters.Protect(noParamOrder, "See docs");

            var pars = PrecleanParams(operation, OprAdd, ui, null, null, parameters, prefill);

            return EntityRule("copy", target, pars, propsKeep: new[] { KeyEntityId, KeyContentType },
                contentType: contentType).Builder;

            //return AddInternal(new ToolbarRuleCopy(target, contentType, 
            //    operation: pars.Operation,// ToolbarRuleOps.Pick(operation, BtnAdd),
            //    pars.Ui, // ObjToString(ui), 
            //    pars.Parameters,// ObjToString(parameters),
            //    GetContext(target, context), 
            //    _deps.ToolbarButtonHelper.Ready));
        }




        public IToolbarBuilder Data(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object filter = null,
            object ui = null,
            object parameters = null,
            string operation = null
        )
        {
            Eav.Parameters.Protect(noParamOrder, "See docs");

            var pars = PrecleanParams(operation, OprAdd, ui, null, null, parameters, null, filter);

            return EntityRule("data", target, pars, propsKeep: new[] { KeyContentType }, contentType: target as string)
                .Builder;

            //var editCommand = new ToolbarRuleForEntity("data", target,
            //    operation: ToolbarRuleOps.Pick(operation, BtnAdd),
            //    ui: ObjToString(ui),
            //    parameters: new ObjectToUrl().SerializeWithChild(parameters, filter, PrefixFilters),
            //    contentType: target as string,
            //    propsKeep: new[] { KeyContentType });
            //return AddInternal(editCommand);
        }

        // TODO: drop image

        [PrivateApi("WIP 13.11")]
        public IToolbarBuilder Image(
            object target,
            string noParamOrder = Eav.Parameters.Protector,
            string ui = null,
            string parameters = null
        )
        {
            Eav.Parameters.Protect(noParamOrder, "See docs");

            return AddInternal(new ToolbarRuleImage(target, ui, parameters, context: GetContext(target, null),
                decoHelper: _deps.ToolbarButtonHelper.Ready));
        }
    }
}
