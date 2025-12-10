namespace ToSic.Sxc.Services.Tweaks.Sys;

/// <summary>
/// WIP 16.08 Helper to let a tweak operation modify a value
/// </summary>
/// <typeparam name="TValue"></typeparam>
[PrivateApi("WIP v17")]
internal record TweakData<TValue>: ITweakData<TValue>
{
    internal TweakData(TValue? initial, string? name, string step, int stepIndex)
    {
        Name = name;
        Step = step;
        Value = initial;
        StepIndex = stepIndex;
    }

    internal TweakData(ITweakData<TValue> original, TValue value, int stepIndex)
    {
        var otw = original as TweakData<TValue>;
        Name = otw?.Name;
        Step = otw?.Step;
        Value = value;
        StepIndex = stepIndex;
    }

    /// <summary>
    /// Name of the value which will be modified, like `FirstName`
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Step of the tweak, like a workflow step. like `before`
    /// </summary>
    public string? Step { get; init; }

    public int StepIndex { get; init; }

    /// <inheritdoc />
    public TValue? Value { get; init; }
}