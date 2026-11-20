using ToSic.Eav.WebApi.Admin.Sys;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using RealController = ToSic.Sxc.Backend.App.CacheControllerReal;

namespace ToSic.Sxc.Dnn.Backend.App;

[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CacheController : DnnApiController, ICacheController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}");

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi);

    #endregion


    private RealController Real => SysHlp.GetService<RealController>();

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
    {
        // Flush-auto needs the current block context
        new DnnSxcWebApiInitBlockContextAndShare().InitBlockSpecs(SysHlp, Request, Log);
        return Real.Flush(appPath, specs);
    }
}
