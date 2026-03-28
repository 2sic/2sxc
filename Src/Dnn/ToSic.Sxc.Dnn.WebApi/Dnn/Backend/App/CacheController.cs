using ToSic.Sxc.Backend.App;
using ToSic.Sxc.Dnn.WebApi.Sys;

namespace ToSic.Sxc.Dnn.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class CacheController() : DnnSxcControllerBase(CacheControllerReal.LogSuffix)
{
    private CacheControllerReal Real => SysHlp.GetService<CacheControllerReal>();

    // Handles the app/auto/cache/flush route, using the current block context unless an appId is supplied.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public bool Flush([FromBody] AppCacheFlushSpecs specs, [FromUri] int? appId = null)
        => Real.FlushAuto(appId, specs);

    // Handles the named-app route app/{appPath}/cache/flush for callers outside of a block context.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public bool Flush([FromUri] string appPath, [FromBody] AppCacheFlushSpecs specs)
        => Real.Flush(appPath, specs);
}
