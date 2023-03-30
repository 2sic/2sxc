using System;
using System.Linq;
using ToSic.Lib.Logging;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Edit.Toolbar.EntityEditInfo;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleForEntity;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOps;


namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private class CleanedParams
        {
            public char Operation;
            public string Ui;
            public string Parameters;
        }

        private (char Operation, string Ui, string Parameters) PreCleanParams(
            string operation, 
            ToolbarRuleOps defOp, 
            object ui, 
            object uiMerge, 
            string uiMergePrefix, 
            object parameters, 
            object prefill,
            object filter = null,
            ITweakButton tweaks = null)
        {
            var paramsString = Utils.PrepareParams(parameters, tweaks);
            var parsWithPrefill = Utils.Prefill2Url.SerializeWithChild(paramsString, prefill, PrefixPrefill);
            return (
                ToolbarRuleOperation.Pick(operation, defOp),
                Ui: PrepareUi(ui, uiMerge, uiMergePrefix, tweaks: tweaks?.UiMerge),
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
                decoHelper: Services.ToolbarButtonHelper.Value);
            var builder = AddInternal(command);
            return (command, builder);
        }

        public IToolbarBuilder Delete(
            object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null)
        {
            Protect(noParamOrder, "See docs");
            var tweaks = RunTweaksOrErrorIfCombined(tweak: tweak, ui: ui, parameters: parameters);

            // Set default operation based on what toolbar is used
            var isDefToolbar = FindRule<ToolbarRuleToolbar>()?.IsDefault ?? false;
            var defOp = isDefToolbar ? OprModify : OprAdd;

            var pars = PreCleanParams(operation, defOp, ui, "show=true", "", parameters, null, tweaks: tweaks);

            return EntityRule("delete", target, pars, 
                propsKeep: new[] { KeyTitle, KeyEntityId, KeyEntityGuid }).Builder;
        }

        public IToolbarBuilder Edit(object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            object prefill = null,
            string operation = null)
        {
            Protect(noParamOrder, "See docs");
            var tweaks = RunTweaksOrErrorIfCombined(tweak: tweak, ui: ui, parameters: parameters, prefill: prefill);
            var pars = PreCleanParams(operation, OprAdd, ui, null, null, parameters, prefill, tweaks: tweaks);

            return EntityRule("edit", target, pars, propsSkip: new[] { KeyEntityGuid, KeyTitle, KeyPublished }).Builder;
        }

        public IToolbarBuilder New(
            object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            object prefill = null,
            string operation = null)
        {
            Protect(noParamOrder, "See docs");
            var tweaks = RunTweaksOrErrorIfCombined(tweak: tweak, ui: ui, parameters: parameters, prefill: prefill);
            var pars = PreCleanParams(operation, OprAdd, ui, null, null, parameters, prefill, tweaks: tweaks);

            return EntityRule("new", target, pars,
                propsSkip: new[] { KeyEntityGuid, KeyEntityId, KeyTitle, KeyPublished },
                contentType: target as string).Builder;
        }

        public IToolbarBuilder Publish(
            object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null)
        {
            Protect(noParamOrder, "See docs");
            var tweaks = RunTweaksOrErrorIfCombined(tweak: tweak, ui: ui, parameters: parameters);
            var pars = PreCleanParams(operation, OprAdd, ui, null, null, parameters, null, tweaks: tweaks);

            return EntityRule("publish", target, pars,
                propsKeep: new[] { KeyEntityId, KeyPublished, KeyIndex, KeyUseModule }).Builder;
        }


        /// <inheritdoc />
        public IToolbarBuilder Metadata(
            object target,
            string contentTypes = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            object prefill = null,
            string operation = null,
            string context = null) => Log.Func(() =>
        {
            Protect(noParamOrder, "See docs");
            var tweaks = RunTweaksOrErrorIfCombined(tweak: tweak, ui: ui, parameters: parameters, prefill: prefill);
            var pars = PreCleanParams(operation, OprAdd, ui, null, null, parameters, prefill, tweaks: tweaks);

            // Note: DO NOT check the target, as here an IAsset is absolutely valid
            // TargetCheck(target);

            var finalTypes = GetMetadataTypeNames(target, contentTypes);
            var realContext = GenerateContext(target, context);
            var builder = this as IToolbarBuilder;


            var mdsToAdd = finalTypes
                .Select(type => new ToolbarRuleMetadata(target, type,
                    operation: pars.Operation,
                    ui: pars.Ui,
                    parameters: pars.Parameters,
                    context: realContext,
                    decoHelper: Services.ToolbarButtonHelper.Value));

            return (builder.AddInternal(mdsToAdd.Cast<object>().ToArray()));
        });

        /// <inheritdoc />
        public IToolbarBuilder Copy(
            object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            string contentType = null,
            object ui = null,
            object parameters = null,
            object prefill = null,
            string operation = null,
            string context = null)
        {
            Protect(noParamOrder, "See docs");
            var tweaks = RunTweaksOrErrorIfCombined(tweak: tweak, ui: ui, parameters: parameters, prefill: prefill);
            var pars = PreCleanParams(operation, OprAdd, ui, null, null, parameters, prefill, tweaks: tweaks);

            return EntityRule("copy", target, pars, propsKeep: new[] { KeyEntityId, KeyContentType },
                contentType: contentType).Builder;
        }




        public IToolbarBuilder Data(
            object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object filter = null,
            object ui = null,
            object parameters = null,
            string operation = null
        )
        {
            Protect(noParamOrder, "See docs");
            var tweaks = RunTweaksOrErrorIfCombined(tweak: tweak, ui: ui, parameters: parameters, filter: filter);
            var pars = PreCleanParams(operation, OprAdd, ui, null, null, parameters, null, filter, tweaks);

            return EntityRule("data", target, pars, propsKeep: new[] { KeyContentType }, contentType: target as string)
                .Builder;
        }
        
    }
}
