using System;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Data;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleForEntity;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        public IToolbarBuilder Settings(
            string noParamOrder = Eav.Parameters.Protector,
            string show = null,
            string hover = null,
            string follow = null,
            string classes = null,
            string autoAddMore = null,
            object ui = null,
            object parameters = null)
            => AddInternal(new ToolbarRuleSettings(show: show, hover: hover, follow: follow, classes: classes, autoAddMore: autoAddMore,
                ui: ObjToString(ui), parameters: ObjToString(parameters)));


        public IToolbarBuilder Parameters(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            object prefill = null,
            string context = null
        )
        {
            TargetCheck(target);
            var clone = new ToolbarBuilder(this);

            // see if we already have a params rule, if yes remove to then later clone and add again
            var previous = clone.FindRule<ToolbarRuleForParams>();
            if (previous != null)
                clone.Rules.Remove(previous);

            // detect if the first parameter (target) is a parameters object
            (target, parameters) = FixTargetIsParameters(target, parameters);

            // Use new or previous target
            target = target ?? previous?.Target;

            // Must create a new one, to not change the original which is still in the original object
            var newParamsRule = new ToolbarRuleForParams(target, 
                O2U.SerializeWithChild(previous?.Ui, ui, ""),
                O2U.SerializeWithChild(O2U.SerializeWithChild(previous?.Parameters, parameters, ""),
                    prefill, PrefixPrefill),
                GetContext(target, context) ?? previous?.Context,
                _deps.ToolbarButtonHelper.Ready);

            clone.Rules.Add(newParamsRule);

            return clone;
        }

        private void TargetCheck(object target)
        {
            if (target is IAsset)
                throw new Exception("Got a target parameter which seems to be an adam-file. " +
                                    "This is not allowed. " +
                                    "Were you trying to target the .Metadata of this file?");

        }

        private (object target, object parameters) FixTargetIsParameters(object target, object parameters)
        {
            // No target, or parameters supplied
            if (parameters != null || target == null) return (target, parameters);

            // Basically only keep the target as is, if it's a known target
            if (target is IEntity || target is IEntityWrapper)
                return (target, null);

            return (null, target);
        }
    }
}
