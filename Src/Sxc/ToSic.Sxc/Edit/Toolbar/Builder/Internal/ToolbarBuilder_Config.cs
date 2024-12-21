using ToSic.Eav.Plumbing;


namespace ToSic.Sxc.Edit.Toolbar.Internal;

partial record ToolbarBuilder
{
    public IToolbarBuilder More(NoParamOrder noParamOrder = default, object ui = default) =>
        this.AddInternal([new ToolbarRuleCustom("more", ui: PrepareUi(ui))]);

    public IToolbarBuilder For(object target) =>
        Parameters(target);

    public IToolbarBuilder DetectDemo(ICanBeEntity root, NoParamOrder noParamOrder = default, string message = default) =>
        this with { Configuration = (Configuration ?? new()) with { DemoCheckItem = root, DemoMessage = message } };

    public IToolbarBuilder Condition(bool condition) =>
        this with { Configuration = (Configuration ?? new()) with { Condition = condition } };

    public IToolbarBuilder Condition(Func<bool> condition) =>
        this with { Configuration = (Configuration ?? new()) with { ConditionFunc = condition } };

    public IToolbarBuilder Audience(NoParamOrder protector = default, bool? everyone = default) =>
        everyone == null ? this : this with { Configuration = (Configuration ?? new()) with { ForceShow = everyone } };

    public IToolbarBuilder Group(string name = null)
    {
        // Auto-determine the group name if none was provided
        // Maybe? only on null, because "" would mean to reset it again?
        if (!name.HasValue())
            name = (Configuration?.Group).HasValue()
                ? Configuration!.Group + "*" // add an uncommon character so each group has another name
                : "custom";

        // Note that we'll add the new buttons directly using AddInternal, so it won't
        // auto-add other UI params such as the previous group
        return name!.StartsWith("-")
            // It's a remove-group rule
            ? this.AddInternal([$"-group={name.Substring(1)}"])
            // It's an add group - set the current group and add the button-rule
            : (this with { Configuration = (Configuration ?? new()) with { Group = name } }).AddInternal([$"+group={name}"]);
    }
}