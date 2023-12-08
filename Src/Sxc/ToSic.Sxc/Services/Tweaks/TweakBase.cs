namespace ToSic.Sxc.Services.Tweaks;

public class TweakBase
{
    public const string StepDefault = "default";
    public const string TargetDefault = "value";

    public string NameId { get; }

    /// <summary>
    /// Name of the target which will be modified, eg `Value`
    /// </summary>
    public string Target { get; }

    /// <summary>
    /// Step of the tweak, like a workflow step. eg. `Result`
    /// </summary>
    public string Step { get; }

    public TweakBase(string nameId, string step = default, string target = null)
    {
        NameId = nameId;
        Step = step ?? StepDefault;
        Target = target ?? TargetDefault;
    }
}