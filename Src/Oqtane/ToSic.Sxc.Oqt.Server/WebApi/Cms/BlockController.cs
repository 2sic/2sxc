using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Cms;
using ToSic.Sxc.WebApi.InPage;
using RealController = ToSic.Sxc.WebApi.Cms.BlockControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms
{
    // Release routes
    [Route(OqtWebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Cms}")]
    [Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Cms}")]
    [Route(OqtWebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Cms}")]

    [ValidateAntiForgeryToken]
    [ApiController]
    // cannot use this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
    public class BlockController : OqtStatefulControllerBase, IBlockController
    {
        public BlockController(): base(RealController.LogSuffix) { }

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
}