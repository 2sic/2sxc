namespace ToSic.Sxc.Services.Tweaks.Sys;

/// <summary>
/// A tweak configuration, describing what to tweak - usually for workflow-style tweaks which execute at certain steps.
/// </summary>
internal record TweakConfig(string NameId)
{
    public const string StepDefault = "default";
    public const string TargetDefault = "value";

    /// <summary>
    /// Identifier of this configuration
    /// </summary>
    public string NameId { get; init; } = NameId;

    /// <summary>
    /// Name of the target which will be modified, like `Value`
    /// </summary>
    [field: AllowNull, MaybeNull]
    public string Target
    {
        get => field ??= TargetDefault;
        init;
    }

    /// <summary>
    /// Step of the tweak, like a workflow step. like `Result`
    /// </summary>
    [field: AllowNull, MaybeNull]
    public string Step
    {
        get => field ??= StepDefault;
        init;
    }
}