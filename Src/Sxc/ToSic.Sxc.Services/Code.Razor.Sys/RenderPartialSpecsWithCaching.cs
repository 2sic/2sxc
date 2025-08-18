using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Services.Cache;

namespace ToSic.Sxc.Code.Razor.Sys;

/// <summary>
/// Extends the PartialSpecs with caching capabilities.
///
/// It is not quite correct in the Services.dll - but this is the best option since it's accessibly by all code which will need it.
/// </summary>
public record RenderPartialSpecsWithCaching: RenderPartialSpecs
{
    /// <summary>
    /// Cache Specs for the partial render.
    /// </summary>
    /// <remarks>
    /// Must allow set, as it is modified at runtime if the razor requests caching.
    /// </remarks>
    public required ICacheSpecs CacheSpecs { get; set; }
}
