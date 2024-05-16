namespace ToSic.Sxc.Edit.Toolbar.Internal;

internal class ToolbarBuilderConfiguration(
    ToolbarBuilderConfiguration original,
    string mode = null,
    bool? condition = null,
    Func<bool> conditionFunc = null,
    bool? forEveryone = null,
    string group = null,
    ICanBeEntity root = default,
    bool? autoDemoMode = default,
    string demoMessage = default)
{
    public readonly string Mode = mode ?? original?.Mode;

    public readonly bool? Condition = condition ?? original?.Condition;

    public readonly Func<bool> ConditionFunc = conditionFunc ?? original?.ConditionFunc;

    // Doesn't seem to be in use ATM
    public readonly bool? ForEveryone = forEveryone ?? original?.ForEveryone;

    public readonly string Group = group ?? original?.Group;

    public readonly ICanBeEntity Root = root ?? original?.Root;

    public readonly string DemoMessage = demoMessage ?? original?.DemoMessage;

    public readonly bool AutoDemoMode = autoDemoMode ?? original?.AutoDemoMode ?? default;

}