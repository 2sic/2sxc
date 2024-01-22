namespace ToSic.Sxc.Edit.Toolbar;

internal class ToolbarBuilderConfiguration(
    ToolbarBuilderConfiguration original,
    string mode = null,
    bool? condition = null,
    Func<bool> conditionFunc = null,
    bool? force = null,
    string group = null,
    ICanBeEntity root = default,
    bool? autoDemoMode = default,
    string demoMessage = default)
{
    public readonly string Mode = mode ?? original?.Mode;

    public readonly bool? Condition = condition ?? original?.Condition;

    public readonly Func<bool> ConditionFunc = conditionFunc ?? original?.ConditionFunc;

    // Doesn't seem to be in use ATM
    public readonly bool? Force = force ?? original?.Force;

    public readonly string Group = group ?? original?.Group;

    public readonly ICanBeEntity Root = root ?? original?.Root;

    public readonly string DemoMessage = demoMessage ?? original?.DemoMessage;

    public readonly bool AutoDemoMode = autoDemoMode ?? original?.AutoDemoMode ?? default;

    // 2022-08-17 2dm - was an idea, but won't work in current infrastructure,
    // because the object doesn't always exist when this code is needed
    //public ToolbarBuilderConfiguration GetUpdated(
    //    string mode = null,
    //    bool? condition = null,
    //    Func<bool> conditionFunc = null,
    //    bool? force = null,
    //    string group = null
    //)
    //{
    //    return null == (mode ?? condition ?? conditionFunc ?? force as object ?? group) 
    //        ? this 
    //        : new ToolbarBuilderConfiguration(this, mode, condition, conditionFunc, force, group);
    //}
}