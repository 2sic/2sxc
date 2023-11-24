using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Dnn.Pages;
using ToSic.Sxc.Dnn.WebApi.Context;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Admin;
using ToSic.Sxc.WebApi.Views;
using RealController = ToSic.Sxc.WebApi.Admin.ViewControllerReal;

namespace ToSic.Sxc.Dnn.WebApi.Admin;

[DnnLogExceptions]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ViewController : SxcApiControllerBase, IViewController
{
    public ViewController() : base(RealController.LogSuffix) { }

    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc />
    [HttpGet]
    [SupportedModules(DnnSupportedModuleNames)]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public IEnumerable<ViewDetailsDto> All(int appId) => Real.All(appId);

    /// <inheritdoc />
    [HttpGet]
    [SupportedModules(DnnSupportedModuleNames)]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public PolymorphismDto Polymorphism(int appId) => Real.Polymorphism(appId);

    /// <inheritdoc />
    [HttpGet, HttpDelete]
    [SupportedModules(DnnSupportedModuleNames)]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public bool Delete(int appId, int id) => Real.Delete(appId, id);

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
        SysHlp.PreventServerTimeout300();
        return Real.Import(new HttpUploadedFile(Request, HttpContext.Current.Request), zoneId, appId);
    }

    /// <inheritdoc />
    [HttpGet]
    [SupportedModules(DnnSupportedModuleNames)]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public IEnumerable<ViewDto> Usage(int appId, Guid guid) => Real.UsagePreparations((views, blocks) =>
    {
        // create array with all 2sxc modules in this portal
        var allMods = new DnnPages(Log).AllModulesWithContent(PortalSettings.PortalId);
        Log.A($"Found {allMods.Count} modules");

        return views.Select(vwb => new ViewDto().Init(vwb, blocks, allMods));
    }).Usage(appId, guid);
}