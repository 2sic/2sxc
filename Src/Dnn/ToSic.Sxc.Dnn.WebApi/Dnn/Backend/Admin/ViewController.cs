using System.Web;
using ToSic.Eav.WebApi.Sys.Context;
using ToSic.Eav.WebApi.Sys.Dto;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Backend.Views;
using ToSic.Sxc.Dnn.Backend.Context;
using ToSic.Sxc.Dnn.Pages;
using ToSic.Sxc.Dnn.WebApi.Sys;
using RealController = ToSic.Sxc.Backend.Admin.ViewControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

[DnnLogExceptions]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ViewController() : DnnSxcControllerBase(RealController.LogSuffix), IViewController
{
    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc />
    [HttpGet]
    [SupportedModules(DnnSupportedModuleNames)]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public IEnumerable<ViewDetailsDto> All(int appId)
        => Real.All(appId);

    /// <inheritdoc />
    [HttpGet]
    [SupportedModules(DnnSupportedModuleNames)]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public PolymorphismDto Polymorphism(int appId)
        => Real.Polymorphism(appId);

    /// <inheritdoc />
    [HttpGet, HttpDelete]
    [SupportedModules(DnnSupportedModuleNames)]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public bool Delete(int appId, int id)
        => Real.Delete(appId, id);

    /// <inheritdoc />
    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public HttpResponseMessage Json(int appId, int viewId)
    {
        // Make sure the Scoped ResponseMaker has this controller context
        SysHlp.SetupResponseMaker(this);

        return Real.Json(appId, viewId);
    }

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public ImportResultDto Import(int zoneId, int appId)
    {
        SysHlp.PreventServerTimeout600();
        return Real.Import(new(Request, HttpContext.Current.Request), zoneId, appId);
    }

    /// <inheritdoc />
    [HttpGet]
    [SupportedModules(DnnSupportedModuleNames)]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public IEnumerable<ViewDto> Usage(int appId, Guid guid)
        => Real.UsagePreparations((views, blocks) =>
            {
                // create array with all 2sxc modules in this portal
                var allMods = new DnnPages(Log).AllModulesWithContent(PortalSettings.PortalId);
                Log.A($"Found {allMods.Count} modules");

                return views.Select(vwb => vwb.Init(blocks, allMods));
            })
            .Usage(appId, guid);
}