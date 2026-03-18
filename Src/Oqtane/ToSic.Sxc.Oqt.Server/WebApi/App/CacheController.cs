using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.Oqt.Server.Controllers;

namespace ToSic.Sxc.Oqt.Server.WebApi.App;

[Route(OqtWebApiConstants.AppRootNoLanguage)]
[Route(OqtWebApiConstants.AppRootPathOrLang)]
[Route(OqtWebApiConstants.AppRootPathAndLang)]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CacheController() : OqtStatefulControllerBase(CacheControllerReal.LogSuffix)
{
    private CacheControllerReal Real => GetService<CacheControllerReal>();

    [HttpPost("{appPath}/{controller}/{action}")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Flush([FromRoute] string appPath, [FromBody] AppCacheFlushRequest request)
        => Real.Flush(appPath, request);

    [HttpPost($"{AppParts.Auto}/{{controller}}/{{action}}")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Flush([FromBody] AppCacheFlushRequest request, [FromQuery] int? appId = null)
        => Real.FlushAuto(request, appId);
}
