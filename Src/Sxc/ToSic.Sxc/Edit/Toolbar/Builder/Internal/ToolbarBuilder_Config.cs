using ToSic.Eav.Plumbing;


namespace ToSic.Sxc.Edit.Toolbar.Internal;

partial class ToolbarBuilder
{
    private IToolbarBuilder With(
        NoParamOrder noParamOrder = default,
        string mode = default,
        object target = default)
    {
        // Create clone before starting to log so it's in there too
        var clone = target == null 
            ? new(this)
            : (ToolbarBuilder)Parameters(target);   // Params will already copy/clone it

        if (mode != null)
            clone.Configuration = (Configuration ?? new()) with { Mode = mode };
        return clone;
    }
    private IToolbarBuilder With(ToolbarBuilderConfiguration configuration)
    {
        // Create clone before starting to log so it's in there too
        var clone = new ToolbarBuilder(this)
        {
            Configuration = configuration
        };

        return clone;
    }
    public IToolbarBuilder More(
        NoParamOrder noParamOrder = default,
        object ui = default
    ) =>
        this.AddInternal(new ToolbarRuleCustom("more", ui: PrepareUi(ui)));

    public IToolbarBuilder For(object target) =>
        With(target: target);

    public IToolbarBuilder DetectDemo(
        ICanBeEntity root,
        NoParamOrder noParamOrder = default,
        string message = default
    ) =>
        With((Configuration ?? new()) with { Root = root, DemoMessage = message });

    public IToolbarBuilder Activate(bool condition)
    {
        throw new NotImplementedException();
    }


    public IToolbarBuilder Condition(bool condition) =>
        With((Configuration ?? new()) with { Condition = condition });

    public IToolbarBuilder Condition(Func<bool> condition) =>
        With((Configuration ?? new()) with { ConditionFunc = condition });

    public IToolbarBuilder Audience(NoParamOrder protector = default, bool? everyone = default) =>
        everyone == null
            ? this
            : With((Configuration ?? new()) with { ForEveryone = everyone });

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
            ? this.AddInternal($"-group={name.Substring(1)}")
            // It's an add group - set the current group and add the button-rule
            : With((Configuration ?? new()) with { Group = name }).AddInternal($"+group={name}");
    }
}