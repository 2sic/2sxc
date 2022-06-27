using System;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {

        public IToolbarBuilder With(
            string noParamOrder = Eav.Parameters.Protector,
            string mode = null,
            object target = null
        ) => WithInternal(noParamOrder, mode, target);

        private IToolbarBuilder WithInternal(
            string noParamOrder = Eav.Parameters.Protector,
            string mode = null,
            object target = null,
            bool? condition = null, 
            Func<bool> conditionFunc = null,
            bool? force = null
        )
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, nameof(WithInternal), 
                $"{nameof(mode)}, {nameof(target)}, {nameof(condition)}, {nameof(conditionFunc)}");
            // Create clone before starting to log so it's in there too
            var clone = new ToolbarBuilder(this);
            var p = clone._params = new ToolbarBuilderParams(_params);
            if (mode != null) p.Mode = mode;
            if (target != null) p.Target = target;
            if (condition != null) p.Condition = condition;
            if (conditionFunc != null) p.ConditionFunc = conditionFunc;
            if (force != null) p.Force = force;
            return clone;
        }

        public IToolbarBuilder Target(object target) => With(target: target);

        public IToolbarBuilder Condition(bool condition) => WithInternal(condition: condition);

        public IToolbarBuilder Condition(Func<bool> condition) => WithInternal(conditionFunc: condition);
    }
}
