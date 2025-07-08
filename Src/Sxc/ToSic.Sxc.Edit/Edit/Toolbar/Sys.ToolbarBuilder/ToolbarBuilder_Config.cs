
using ToSic.Sxc.Edit.Toolbar.Sys.Rules;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;

partial record ToolbarBuilder
{
    public IToolbarBuilder More(NoParamOrder noParamOrder = default, object? ui = default)
        => this.AddInternal([new ToolbarRuleCustom("more", ui: PrepareUi(ui))]);

    public IToolbarBuilder For(object target)
        => Parameters(target);

    public IToolbarBuilder DetectDemo(ICanBeEntity root, NoParamOrder noParamOrder = default, string? message = default)
        => this with { Configuration = Configuration with { DemoCheckItem = root, DemoMessage = message } };

    public IToolbarBuilder Condition(bool condition)
        => this with { Configuration = Configuration with { Condition = condition } };

    public IToolbarBuilder Condition(Func<bool> condition)
        => this with { Configuration = Configuration with { ConditionFunc = condition } };

    public IToolbarBuilder Audience(
        NoParamOrder protector = default,
        bool? everyone = default,
        IEnumerable<string>? roleNames = default,
        IEnumerable<string>? denyRoleNames = default)
    {
        if (everyone == null && roleNames == null && denyRoleNames == null)
            return this;

        var config = Configuration;

        if (everyone != null)
            config = config with { ShowForEveryone = everyone };
        if (roleNames != null)
            config = config with { ShowForRoles = roleNames.ToList() };
        if (denyRoleNames != null)
            config = config with { ShowDenyRoles = denyRoleNames.ToList() };

        return this with { Configuration = config };
    }

    public IToolbarBuilder Group(string? name = null)
    {
        // Auto-determine the group name if none was provided
        // Maybe? only on null, because "" would mean to reset it again?
        if (name.IsEmpty())
            name = Configuration.Group.HasValue()
                ? Configuration.Group + "*" // add an uncommon character so each group has another name
                : "custom";

        // Note that we'll add the new buttons directly using AddInternal, so it won't
        // auto-add other UI params such as the previous group
        return name.StartsWith("-")
            // It's a remove-group rule
            ? this.AddInternal([new ToolbarRuleGeneric($"-group={name.Substring(1)}")])
            // It's an add group - set the current group and add the button-rule
            : (this with { Configuration = Configuration with { Group = name } })
            .AddInternal([new ToolbarRuleGeneric($"+group={name}")]);
    }
}