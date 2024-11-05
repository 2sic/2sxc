using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using ToSic.Lib.DI;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Backend.Views;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Sxc.Backend.Admin.ViewControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

[AllowAnonymous] // necessary at this level, because otherwise download would fail

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Admin}")]

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ViewController : OqtStatefulControllerBase, IViewController
{
    public ViewController(LazySvc<Pages.Pages> pages) : base(RealController.LogSuffix)
    {
        this.ConnectLogs([
            _pages = pages
        ]);
    }
    private readonly LazySvc<Pages.Pages> _pages;

    private RealController Real => GetService<RealController>();

    /// <inheritdoc />
    [HttpGet]
    //[SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public IEnumerable<ViewDetailsDto> All(int appId) => Real.All(appId);

    /// <inheritdoc />
    [HttpGet]
    //[SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public PolymorphismDto Polymorphism(int appId) => Real.Polymorphism(appId);

    /// <inheritdoc />
    [HttpGet, HttpDelete]
    //[SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Delete(int appId, int id) => Real.Delete(appId, id);

    /// <inheritdoc />
    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public IActionResult Json(int appId, int viewId)
    {
        // Make sure the Scoped ResponseMaker has this controller context
        CtxHlp.SetupResponseMaker();
        return Real.Json(appId, viewId);
    }

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public ImportResultDto Import(int zoneId, int appId) => Real.Import(new(Request), zoneId, appId);

    /// <inheritdoc />
    [HttpGet]
    //[SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public IEnumerable<ViewDto> Usage(int appId, Guid guid) => Real.UsagePreparations((views, blocks) =>
    {
        // create list with all 2sxc modules in this site
        var allMods = _pages.Value.AllModulesWithContent(Real.SiteId);
        Log.A($"Found {allMods.Count} modules");

        return views.Select(vwb => _pages.Value.ViewDtoBuilder(vwb, blocks, allMods));
    }).Usage(appId, guid);


}