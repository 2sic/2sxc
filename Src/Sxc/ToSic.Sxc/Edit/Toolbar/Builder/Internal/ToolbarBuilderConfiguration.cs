namespace ToSic.Sxc.Edit.Toolbar.Internal;

internal record ToolbarBuilderConfiguration
{
    public string Mode { get; init; }

    public bool? Condition { get; init; }

    public Func<bool> ConditionFunc { get; init; }

    // Doesn't seem to be in use ATM
    public bool? ForEveryone { get; init; }

    public string Group { get; init; }

    public ICanBeEntity Root { get; init; }

    public string DemoMessage { get; init; }

    public bool AutoDemoMode { get; init; }
}