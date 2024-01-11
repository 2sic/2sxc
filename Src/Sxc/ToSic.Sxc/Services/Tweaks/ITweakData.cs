namespace ToSic.Sxc.Services.Tweaks;

/// <summary>
/// Container for a value (data) to tweak.
/// As of now just contains the value in a property, but will be extended in the future.
/// So in future it could have more context information etc.
/// </summary>
/// <remarks>New in v17 - NOT YET IN USE IN PUBLIC APIs</remarks>
/// <typeparam name="TValue"></typeparam>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal interface ITweakData<out TValue>
{
    /// <summary>
    /// Current value before tweaking.
    /// </summary>
    TValue Value { get; }
}