using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Admin.Sys;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.Sys.ActionFilters;

namespace ToSic.Sxc.Oqt.Server.WebApi.App;

[Route(OqtWebApiConstants.AppRootNoLanguage)]
[Route(OqtWebApiConstants.AppRootPathOrLang)]
[Route(OqtWebApiConstants.AppRootPathAndLang)]
[OqtApiRequest]
[SystemTextJsonFormatter]
[ServiceFilter(typeof(OptionalBodyFilter))]
[ServiceFilter(typeof(HttpResponseExceptionFilter))]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CacheController : ControllerBase, IHasLog, ICacheController
{
    /// <inheritdoc />
    public ILog Log => field ??= LogFactory.Create($"Api.{CacheControllerReal.LogSuffix}", null, GetType().Name);

    private CacheControllerReal Real => this.GetApiService<CacheControllerReal>();

    [HttpPost("{appPath}/{controller}/{action}")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Flush([FromRoute] string appPath, [FromBody] AppCacheFlushSpecs specs)
        => Real.Flush(appPath, specs);

    [HttpPost($"{AppParts.Auto}/{{controller}}/{{action}}")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Flush([FromBody] AppCacheFlushSpecs specs, [FromQuery] int? appId = null)
    {
        if (appId == null)
            this.PrepareRequestBlock();
        return Real.FlushAuto(appId, specs);
    }
}
