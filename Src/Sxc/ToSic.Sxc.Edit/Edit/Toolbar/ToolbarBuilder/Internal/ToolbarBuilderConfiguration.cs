namespace ToSic.Sxc.Edit.Toolbar.Internal;

internal record ToolbarBuilderConfiguration
{
    /// <summary>
    /// How the HTML is created - for tag, attribute, etc.
    /// </summary>
    public string? HtmlMode { get; init; }

    /// <summary>
    /// Condition to choose if this toolbar should be rendered at all.
    /// </summary>
    public bool? Condition { get; init; }

    /// <summary>
    /// Condition to choose if this toolbar should be rendered at all.
    /// </summary>
    public Func<bool>? ConditionFunc { get; init; }

    /// <summary>
    /// Setting from toolbar API.
    /// </summary>
    public bool? ShowForEveryone { get; init; }

    /// <summary>
    /// Final effect, which will be reviewed when showing the toolbar; set/read internally.
    /// </summary>
    internal bool? ShowForce { get; init; }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>New v20</remarks>
    public List<string>? ShowForRoles { get; init; }

    public List<string>? ShowDenyRoles { get; init; }

    /// <summary>
    /// Temporary group identifier, which is used for all following buttons which are specified.
    /// </summary>
    public string? Group { get; init; }

    /// <summary>
    /// Item to check if the toolbar should be in demo mode.
    /// </summary>
    public ICanBeEntity? DemoCheckItem { get; init; }

    /// <summary>
    /// Message to show if the toolbar is in demo mode.
    /// </summary>
    public string? DemoMessage { get; init; }

}