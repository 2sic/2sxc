using ToSic.Sxc.Services.Cache;

namespace ToSic.Sxc.Custom.Hybrid;

/// <summary>
/// Configure Razor - for example output caching.
/// </summary>
public interface IRazorConfiguration
{
    /// <summary>
    /// Configure output of Razor partials - especially caching.
    /// </summary>
    /// <param name="protector"></param>
    /// <param name="cache"></param>
    /// <returns>always returns `null` so it can be used inline in Razor.</returns>
    /// <remarks>
    /// Will only hav an effect if the feature [LightSpeedOutputCachePartials](https://patrons.2sxc.org/features/feat/LightSpeedOutputCachePartials) is enabled.
    /// </remarks>
    string Partial(NoParamOrder protector = default, Func<ICacheSpecs, ICacheSpecs> cache = default);

    string PartialCache(NoParamOrder protector = default, int? sliding = null, string watch = null, string varyBy = null, string url = null, string model = null);
}