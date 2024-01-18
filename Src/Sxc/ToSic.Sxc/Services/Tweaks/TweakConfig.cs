namespace ToSic.Sxc.Services.Tweaks;

internal class TweakConfig(string nameId, string step = default, string target = null)
{
    public const string StepDefault = "default";
    public const string TargetDefault = "value";

    public string NameId { get; } = nameId;

    /// <summary>
    /// Name of the target which will be modified, eg `Value`
    /// </summary>
    public string Target { get; } = target ?? TargetDefault;

    /// <summary>
    /// Step of the tweak, like a workflow step. eg. `Result`
    /// </summary>
    public string Step { get; } = step ?? StepDefault;
}