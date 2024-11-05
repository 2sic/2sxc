using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using ToSic.Eav.Apps.Internal.Ui;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Backend.Cms;
using ToSic.Sxc.Backend.InPage;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using RealController = ToSic.Sxc.Backend.Cms.BlockControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms;

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Cms}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Cms}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Cms}")]

[ValidateAntiForgeryToken]
[ApiController]
// cannot use this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class BlockController() : OqtStatefulControllerBase(RealController.LogSuffix), IBlockController
{
    private RealController Real => GetService<RealController>();



    /// <inheritdoc />
    [HttpPost]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public string Block(int parentId, string field, int index, string app = "", Guid? guid = null)
        => Real.Block(parentId, field, index, app, guid);

    /// <inheritdoc />
    [HttpPost]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public void Item(int? index = null) => Real.Item(index);


    /// <inheritdoc />
    [HttpPost]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public void App(int? appId) => Real.App(appId);

    /// <inheritdoc />
    [HttpGet]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public IEnumerable<AppUiInfo> Apps(string apps = null) => Real.Apps(apps);


    /// <inheritdoc />
    [HttpGet]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public IEnumerable<ContentTypeUiInfo> ContentTypes() => Real.ContentTypes();

    /// <inheritdoc />
    [HttpGet]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public IEnumerable<TemplateUiInfo> Templates() => Real.Templates();

    /// <inheritdoc />
    [HttpPost]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    //[Authorize(Roles = RoleNames.Registered)]
    [Authorize(Roles = RoleNames.Admin)]
    // TODO: 2DM please check permissions
    public Guid? Template(int templateId, bool forceCreateContentGroup) => Real.Template(templateId, forceCreateContentGroup);


    /// <inheritdoc />
    [HttpGet]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public AjaxRenderDto Render(int templateId, string lang, string edition) => Real.Set(OqtConstants.UiRoot).Render(templateId, lang, edition);

    /// <inheritdoc />
    [HttpPost]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Publish(string part, int index) => Real.Publish(part, index);

}