using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Services.Cache;

namespace ToSic.Sxc.Web.Sys.LightSpeed;

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
