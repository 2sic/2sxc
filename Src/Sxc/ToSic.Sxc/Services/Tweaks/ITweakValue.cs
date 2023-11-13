namespace ToSic.Sxc.Services.Tweaks
{
    /// <summary>
    /// Container for a value to tweak.
    /// As of now just contains the value in a property, but will be extended in the future.
    /// So in future it could have more context information etc.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface ITweakValue<out TValue>
    {
        /// <summary>
        /// Current value
        /// </summary>
        TValue Value { get; }
    }
}
