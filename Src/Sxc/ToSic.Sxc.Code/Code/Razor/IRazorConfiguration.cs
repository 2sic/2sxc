using ToSic.Sxc.Services.Cache;

namespace ToSic.Sxc.Code.Razor;

/// <summary>
/// Configure Razor - for example output caching.
/// </summary>
public interface IRazorConfiguration
{
    /// <summary>
    /// Configure output of Razor partials - especially caching. Note that for caching only, the shorter <see cref="PartialCache"/> is recommended and will also apply defaults.
    /// </summary>
    /// <param name="protector"></param>
    /// <param name="tweak"></param>
    /// <returns>always returns `null` so it can be used inline in Razor.</returns>
    /// <remarks>
    /// Will only hav an effect if the feature [LightSpeedOutputCachePartials](https://patrons.2sxc.org/features/feat/LightSpeedOutputCachePartials) is enabled.
    /// </remarks>
    string? PartialCache(NoParamOrder protector = default,
        int? sliding = null, // legacy name
        int? seconds = null,
        string? watch = null,
        string? varyBy = null,
        string? url = null,
        string? model = null,
        Func<ICacheSpecs, ICacheSpecs>? tweak = default);
}