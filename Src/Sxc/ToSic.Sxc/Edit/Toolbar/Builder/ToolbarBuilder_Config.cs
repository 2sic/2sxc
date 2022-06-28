using System;
using ToSic.Eav;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private IToolbarBuilder WithInternal(
            string noParamOrder = Parameters.Protector,
            string mode = null,
            object target = null,
            bool? condition = null, 
            Func<bool> conditionFunc = null,
            bool? force = null
        )
        {
            Parameters.Protect(noParamOrder, $"{nameof(mode)}, {nameof(target)}, {nameof(condition)}, {nameof(conditionFunc)}");
            // Create clone before starting to log so it's in there too
            var clone = new ToolbarBuilder(this);
            var p = clone._params = new ToolbarBuilderParams(_params);
            if (mode != null) p.Mode = mode;
            if (target != null)
            {
                p.Target = target;
                // see if we already have a params rule
                var existingParamsRule = clone.FindRule<ToolbarRuleForParams>();
                if (existingParamsRule != null) 
                    clone.Rules.Remove(existingParamsRule);
                // Must create a new one, to not change the original which is still in the original object
                var newParamsRule = new ToolbarRuleForParams(target, existingParamsRule?.Ui,
                    existingParamsRule?.Parameters, existingParamsRule?.Context);

                clone.Rules.Add(newParamsRule);
            }
            if (condition != null) p.Condition = condition;
            if (conditionFunc != null) p.ConditionFunc = conditionFunc;
            if (force != null) p.Force = force;
            return clone;
        }

        public IToolbarBuilder More(
            string noParamOrder = Parameters.Protector,
            object ui = null
        )
        {
            Parameters.Protect(noParamOrder, nameof(ui));
            return AddInternal(new ToolbarRuleCustom("more", ui: ObjToString(ui)));
        }

        public IToolbarBuilder Target(object target) => WithInternal(target: target);

        public IToolbarBuilder Condition(bool condition) => WithInternal(condition: condition);

        public IToolbarBuilder Condition(Func<bool> condition) => WithInternal(conditionFunc: condition);
    }
}
