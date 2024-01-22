using ToSic.Sxc.Code.Internal.Documentation;

// Important
// This is just the public API for this
// Many other APIs simply shouldn't be public, so the Eav-version isn't fully surfaced here

namespace ToSic.Sxc.Services;

/// <summary>
/// Features lets your code find out what system features are currently enabled/disabled in the environment.
/// It's important to detect if the admin must activate certain features to let your code do it's work.
/// </summary>
/// <remarks>
/// This replaces the older static Features accessor - please only use this from now on.
///
/// History:
/// - Added this implementation in 13.01
/// </remarks>
[PublicApi]
[Docs(AutoLink = true, Messages = ["Service to verify if a feature is enabled."])]
public interface IFeaturesService: IHasLog, ICanDebug
{
    /// <summary>
    /// Checks if a list of features are enabled, in case you need many features to be activated.
    /// </summary>
    /// <param name="nameIds">one or many name IDs - can also be the guids (as string)</param>
    /// <returns>true if all features are enabled, false if any one of them is not</returns>
    /// <remarks>
    /// Added in v13.01
    /// </remarks>
    bool IsEnabled(params string[] nameIds);
}