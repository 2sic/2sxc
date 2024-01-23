namespace ToSic.Sxc.Context.Internal;

/// <summary>
/// General platform information
/// </summary>
/// <remarks>
/// This must be provided through Dependency Injection, Singleton, as it cannot change at runtime.
/// </remarks>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal abstract class Platform: IPlatform
{

    /// <summary>
    /// The platform type Id from the enumerator - so stored as an int.
    /// </summary>
    public abstract PlatformType Type { get; }

    /// <summary>
    /// A nice name ID, like "Dnn" or "Oqtane"
    /// </summary>
    public string Name => Type.ToString();

    public abstract Version Version { get; }
}