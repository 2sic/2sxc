

using ToSic.Eav.WebApi.Admin.Sys;
#if NETFRAMEWORK
using System.Web.Http;
using FromRoute = System.Web.Http.FromUriAttribute;
#else
using Microsoft.AspNetCore.Mvc;
#endif

namespace ToSic.Sxc.Backend.OutputCache;

/// <summary>
/// Output Cache Controller. SEEMS TO BE UNUSED #OutputCacheControllerSeemsUnused2026-11-21
/// </summary>
/// <remarks>
/// As of 2026-11 2dm believes that this is completely unused.
/// My guess is that a long time ago we didn't have automatic cache flushing on data change / file change.
/// So we probably had a button to do this.
/// But this must be very long ago, so ATM I plan to comment everything out.
///
/// If we decide to reactivate it, it should be renamed to IOutputCacheController and the
/// route should also be .../outputCache/...
/// </remarks>
public interface ICacheController: IAppAdminWebApiController
{
    // UI test: Not called by eav-ui; exercise admin/cache/flush with an appId directly.
    bool Flush([FromBody] AppCacheFlushSpecs specs, [FromRoute] int? appId = null);
    // UI test: Not called by eav-ui; exercise admin/cache/flush with an appPath directly.
    bool Flush([FromRoute] string appPath, [FromBody] AppCacheFlushSpecs specs);
}
