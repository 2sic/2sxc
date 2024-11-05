using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Sxc.Backend.Admin.TypeControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

/// <summary>
/// This one supplies portal-wide (or cross-portal) settings / configuration
/// </summary>
//[SupportedModules("2sxc,2sxc-app")]
//[DnnLogExceptions]
[Authorize(Roles = RoleNames.Admin)]
[AutoValidateAntiforgeryToken]

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Admin}")]


[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class TypeController() : OqtStatefulControllerBase(RealController.LogSuffix), ITypeController
{
    private RealController Real => GetService<RealController>();


    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public IEnumerable<ContentTypeDto> List(int appId, string scope = null, bool withStatistics = false) => Real.List(appId, scope, withStatistics);


    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public ScopesDto Scopes(int appId) => Real.Scopes(appId);


    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public ContentTypeDto Get(int appId, string contentTypeId, string scope = null) => Real.Get(appId, contentTypeId, scope);


    [HttpDelete]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Delete(int appId, string staticName) => Real.Delete(appId, staticName);


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Save(int appId, [FromBody] Dictionary<string, object> item) => Real.Save(appId, item);


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Host)]
    public bool AddGhost(int appId, string sourceStaticName) => Real.AddGhost(appId, sourceStaticName);


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public void SetTitle(int appId, int contentTypeId, int attributeId) => Real.SetTitle(appId, contentTypeId, attributeId);


    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public IActionResult Json(int appId, string name)
    {
        // Make sure the Scoped ResponseMaker has this controller context
        CtxHlp.SetupResponseMaker();
        return Real.Json(appId, name);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public ImportResultDto Import(int zoneId, int appId) => Real.Import(new(Request), zoneId, appId);


    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public IActionResult JsonBundleExport(int appId, Guid exportConfiguration, int indentation = 0)
    {
        // Make sure the Scoped ResponseMaker has this controller context
        CtxHlp.SetupResponseMaker();
        return Real.JsonBundleExport(appId, exportConfiguration, indentation);
    }

}