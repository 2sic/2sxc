namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// Carries information about what compatibility level to use. Important for components that have an older and newer API.
/// </summary>
[PrivateApi("this is just fyi, was published as internal till v14")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ICompatibilityLevel
{
    /// <summary>
    /// The compatibility level to use.
    /// Will affect what features Razor etc. will provide.
    /// </summary>
    int CompatibilityLevel { get; }
}