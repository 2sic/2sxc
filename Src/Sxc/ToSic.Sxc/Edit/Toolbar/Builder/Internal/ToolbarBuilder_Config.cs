using ToSic.Eav.Plumbing;


namespace ToSic.Sxc.Edit.Toolbar.Internal;

partial class ToolbarBuilder
{
    private IToolbarBuilder With(
        NoParamOrder noParamOrder = default,
        string mode = default,
        object target = default,
        bool? condition = default, 
        Func<bool> conditionFunc = default,
        bool? forEveryone = default,
        string group = default,
        ICanBeEntity root = default,
        bool? autoDemoMode = default,
        string demoMessage = default
    )
    {
        // Create clone before starting to log so it's in there too
        var clone = target == null 
            ? new(this)
            : (ToolbarBuilder)Parameters(target);   // Params will already copy/clone it

        clone.Configuration = new(
            Configuration,
            mode: mode, 
            condition: condition, 
            conditionFunc: conditionFunc, 
            forEveryone: forEveryone, 
            group: group,
            root: root,
            autoDemoMode: autoDemoMode,
            demoMessage: demoMessage
        );
        return clone;
    }

    public IToolbarBuilder More(
        NoParamOrder noParamOrder = default,
        object ui = default
    ) =>
        this.AddInternal(new ToolbarRuleCustom("more", ui: PrepareUi(ui)));

    public IToolbarBuilder For(object target) => With(target: target);

    public IToolbarBuilder DetectDemo(
        ICanBeEntity root,
        NoParamOrder noParamOrder = default,
        string message = default)
        => With(root: root, demoMessage: message);

    public IToolbarBuilder Condition(bool condition) => With(condition: condition);

    public IToolbarBuilder Condition(Func<bool> condition) => With(conditionFunc: condition);

    public IToolbarBuilder Audience(NoParamOrder protector = default, bool? everyone = default) 
        => everyone == null ? this : With(forEveryone: everyone);

    public IToolbarBuilder Group(string name = null)
    {
        // Auto-determine the group name if none was provided
        // Maybe? only on null, because "" would mean to reset it again?
        if (!name.HasValue())
            name = Configuration?.Group.HasValue() == true
                ? Configuration?.Group + "*" // add an uncommon character so each group has another name
                : "custom";

        // Note that we'll add the new buttons directly using AddInternal, so it won't
        // auto-add other UI params such as the previous group
        return name.StartsWith("-")
            // It's a remove-group rule
            ? this.AddInternal($"-group={name.Substring(1)}") 
            // It's an add group - set the current group and add the button-rule
            : With(group: name).AddInternal($"+group={name}");
    }
}