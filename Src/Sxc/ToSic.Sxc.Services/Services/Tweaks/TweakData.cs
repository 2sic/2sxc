namespace ToSic.Sxc.Services.Tweaks;

/// <summary>
/// WIP 16.08 Helper to let a tweak operation modify a value
/// </summary>
/// <typeparam name="TValue"></typeparam>
[PrivateApi("WIP v17")]
internal class TweakData<TValue>: ITweakData<TValue>
{
    internal TweakData(TValue initial, string name, string step, int stepIndex)
    {
        Name = name;
        Step = step;
        //Initial = initial;
        Value = initial;
        StepIndex = stepIndex;
    }

    internal TweakData(ITweakData<TValue> original, TValue value, int stepIndex)
    {
        var otw = original as TweakData<TValue>;
        Name = otw?.Name;
        Step = otw?.Step;
        //Initial = original.Initial;
        Value = value;
        StepIndex = stepIndex;
    }

    /// <summary>
    /// Name of the value which will be modified, like `FirstName`
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Step of the tweak, like a workflow step. like `before`
    /// </summary>
    public string Step { get; }

    public int StepIndex { get; }

    ///// <summary>
    ///// Initial value before any processing had happened
    ///// </summary>
    //public TValue Initial { get; }

    /// <inheritdoc />
    public TValue Value { get; }

    ///// <summary>
    ///// Get a new TweakValue with the value changed.
    ///// This is so that the object is always immutable.
    ///// </summary>
    ///// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    ///// <param name="value"></param>
    ///// <returns></returns>
    //public TweakValue<TValue> New(NoParamOrder noParamOrder = default, TValue value = default)
    //    => new(this, value);
}