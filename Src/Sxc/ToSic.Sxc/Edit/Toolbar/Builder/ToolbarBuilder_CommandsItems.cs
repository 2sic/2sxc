using System.Linq;
using ToSic.Lib.Logging;
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
            var paramsString = Utils.Par2Url.Serialize(parameters);
            var parsWithPrefill = Utils.Prefill2Url.SerializeWithChild(paramsString, prefill, PrefixPrefill);
            return (
                ToolbarRuleOperation.Pick(operation, defOp),
                Ui: PrepareUi(ui, uiMerge, uiMergePrefix),
                Parameters: Utils.Filter2Url.SerializeWithChild(parsWithPrefill, filter, PrefixFilters)
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
            TargetCheck(target);
            var command = new ToolbarRuleForEntity(verb, target, pars.Operation,
                ui: pars.Ui, parameters: pars.Parameters,
                contentType: contentType,
                propsKeep: propsKeep, propsSkip: propsSkip,
                decoHelper: Deps.ToolbarButtonHelper.Value);
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
            string context = null) => Log.Func(() =>
        {
            Eav.Parameters.Protect(noParamOrder, "See docs");

            // Note: DO NOT check the target, as here an IAsset is absolutely valid
            // TargetCheck(target);

            var finalTypes = GetMetadataTypeNames(target, contentTypes);
            var realContext = GenerateContext(target, context);
            var builder = this as IToolbarBuilder;

            var pars = PrecleanParams(operation, OprAdd, ui, null, null, parameters, prefill);

            var mdsToAdd = finalTypes
                .Select(type => new ToolbarRuleMetadata(target, type,
                    operation: pars.Operation,
                    ui: pars.Ui,
                    parameters: pars.Parameters,
                    context: realContext,
                    decoHelper: Deps.ToolbarButtonHelper.Value));

            return (builder.AddInternal(mdsToAdd.Cast<object>().ToArray()));
        });

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
        }
        
    }
}
