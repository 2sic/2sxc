using ToSic.Sxc.Services.Cache;

namespace ToSic.Sxc.Code.Razor;

/// <summary>
/// Configure Razor - for example output caching.
/// </summary>
public interface IRazorConfiguration
{
    /// <summary>
    /// Configure output of Razor partials caching.
    /// </summary>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="seconds">sliding seconds to keep the cache, like `60`, `300`, `3600`</param>
    /// <param name="watch">what to watch for to flush the cache - recommended: `data,folder`</param>
    /// <param name="varyBy">what to vary the cache by, like `user,module`</param>
    /// <param name="model">when caching by model properties, the model property names like `id,key`</param>
    /// <param name="url">url parameters to vary by</param>
    /// <param name="tweak">extended / custom configuration of the cache - use for advanced config like elevation based variants</param>
    /// <returns>always returns `null` so it can be used inline in Razor.</returns>
    /// <remarks>
    /// Will only hav an effect if the feature [LightSpeedOutputCachePartials](https://patrons.2sxc.org/features/feat/LightSpeedOutputCachePartials) is enabled.
    /// </remarks>
    string? PartialCache(NoParamOrder npo = default,
        int? seconds = null,
        string? watch = null,
        string? varyBy = null,
        string? url = null,
        string? model = null,
        Func<ICacheSpecs, ICacheSpecs>? tweak = default);
}