using ToSic.Sxc.Services.OutputCache;

namespace ToSic.Sxc.Render.Sys.ModuleHtml;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
/// <summary>
/// Temporary per-module buffer for output-cache configuration collected during a single render.
/// It exists because OutputCache.Configure(...) and OutputCache.DependOn(...) may happen at different times,
/// but the final merged settings are only needed when the render result is finalized.
/// </summary>
internal sealed class ModuleOutputCacheState
{
    // Last explicit OutputCache.Configure(...) settings captured for this module render.
    public OutputCacheSettings? Settings { get; set; }

    // Union of all DependOn(...) keys collected while the module is rendering.
    public HashSet<string> ExternalDependencyKeys { get; } = new(StringComparer.OrdinalIgnoreCase);
}
