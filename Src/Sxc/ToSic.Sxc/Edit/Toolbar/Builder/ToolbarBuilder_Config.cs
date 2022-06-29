using System;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private IToolbarBuilder With(
            string noParamOrder = Eav.Parameters.Protector,
            string mode = null,
            object target = null,
            bool? condition = null, 
            Func<bool> conditionFunc = null,
            bool? force = null
        )
        {
            Eav.Parameters.Protect(noParamOrder, $"{nameof(mode)}, {nameof(target)}, {nameof(condition)}, {nameof(conditionFunc)}");
            // Create clone before starting to log so it's in there too
            var clone = target == null 
                ? new ToolbarBuilder(this)
                : (ToolbarBuilder)Parameters(target);   // Params will already copy/clone it

            var p = clone._configuration = new ToolbarBuilderConfiguration(_configuration);
            if (mode != null) p.Mode = mode;
            if (condition != null) p.Condition = condition;
            if (conditionFunc != null) p.ConditionFunc = conditionFunc;
            if (force != null) p.Force = force;
            return clone;
        }

        public IToolbarBuilder More(
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null
        )
        {
            Eav.Parameters.Protect(noParamOrder, nameof(ui));
            return AddInternal(new ToolbarRuleCustom("more", ui: ObjToString(ui)));
        }

        public IToolbarBuilder For(object target) => With(target: target);

        public IToolbarBuilder Target(object target) => With(target: target);

        public IToolbarBuilder Condition(bool condition) => With(condition: condition);

        public IToolbarBuilder Condition(Func<bool> condition) => With(conditionFunc: condition);
    }
}
