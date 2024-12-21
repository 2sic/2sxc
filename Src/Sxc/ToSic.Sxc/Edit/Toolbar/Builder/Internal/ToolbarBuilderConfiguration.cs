namespace ToSic.Sxc.Edit.Toolbar.Internal;

internal record ToolbarBuilderConfiguration
{
    /// <summary>
    /// How the HTML is created - for tag, attribute, etc.
    /// </summary>
    public string HtmlMode { get; init; }

    /// <summary>
    /// Condition to choose if this toolbar should be rendered at all.
    /// </summary>
    public bool? Condition { get; init; }

    /// <summary>
    /// Condition to choose if this toolbar should be rendered at all.
    /// </summary>
    public Func<bool> ConditionFunc { get; init; }

    public bool? ForceShow { get; init; }

    /// <summary>
    /// Temporary group identifier, which is used for all following buttons which are specified.
    /// </summary>
    public string Group { get; init; }

    /// <summary>
    /// Item to check if the toolbar should be in demo mode.
    /// </summary>
    public ICanBeEntity DemoCheckItem { get; init; }

    /// <summary>
    /// Message to show if the toolbar is in demo mode.
    /// </summary>
    public string DemoMessage { get; init; }

}