using System;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private IToolbarBuilder With(
            string noParamOrder = Eav.Parameters.Protector,
            string mode = default,
            object target = default,
            bool? condition = default, 
            Func<bool> conditionFunc = default,
            bool? force = default,
            string group = default,
            ICanBeEntity root = default,
            bool? autoDemoMode = default,
            string demoMessage = default
        )
        {
            Eav.Parameters.Protect(noParamOrder, $"{nameof(mode)}, {nameof(target)}, {nameof(condition)}, {nameof(conditionFunc)}");
            // Create clone before starting to log so it's in there too
            var clone = target == null 
                ? new ToolbarBuilder(this)
                : (ToolbarBuilder)Parameters(target);   // Params will already copy/clone it

            var p = clone._configuration = new ToolbarBuilderConfiguration(
                _configuration,
                mode: mode, 
                condition: condition, 
                conditionFunc: conditionFunc, 
                force: force, 
                group: group,
                root: root,
                autoDemoMode: autoDemoMode,
                demoMessage: demoMessage
            );
            return clone;
        }

        public IToolbarBuilder More(
            string noParamOrder = Eav.Parameters.Protector,
            object ui = default
        )
        {
            Eav.Parameters.Protect(noParamOrder, nameof(ui));
            return this.AddInternal(new ToolbarRuleCustom("more", ui: PrepareUi(ui)));
        }

        public IToolbarBuilder For(object target) => With(target: target);

        public IToolbarBuilder DetectDemo(
            ICanBeEntity root,
            string noParamOrder = Protector,
            string message = default)
            => With(root: root, demoMessage: message);

        public IToolbarBuilder Condition(bool condition) => With(condition: condition);

        public IToolbarBuilder Condition(Func<bool> condition) => With(conditionFunc: condition);

        public IToolbarBuilder Group(string name = null)
        {
            // Auto-determine the group name if none was provided
            // Maybe? only on null, because "" would mean to reset it again?
            if (!name.HasValue())
                name = _configuration?.Group.HasValue() == true
                    ? _configuration?.Group + "*" // add a uncommon character so each group has another name
                    : "custom";

            // Note that we'll add the new buttons directly using AddInternal so it won't
            // auto-add other UI params such as the previous group
            return name.StartsWith("-")
                // It's a remove-group rule
                ? this.AddInternal($"-group={name.Substring(1)}") 
                // It's an add group - set the current group and add the button-rule
                : With(group: name).AddInternal($"+group={name}");
        }
    }
}
