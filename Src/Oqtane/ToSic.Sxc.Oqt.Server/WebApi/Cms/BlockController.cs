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

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Cms}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Cms}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Cms}")]

    [ValidateAntiForgeryToken]
    [ApiController]
    // cannot use this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
    public class BlockController : OqtStatefulControllerBase<BlockControllerReal>, IBlockController
    {
        public BlockController(): base(BlockControllerReal.LogSuffix) { }


        #region Block

        /// <inheritdoc />
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public string Block(int parentId, string field, int sortOrder, string app = "", Guid? guid = null)
            => Real.Block(parentId, field, sortOrder, app, guid);
        #endregion

        #region BlockItems
        /// <summary>
        /// used to be GET Module/AddItem
        /// </summary>
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public void Item(int? index = null) => Real.Item(index);

        #endregion


        #region App

        /// <summary>
        /// used to be GET Module/SetAppId
        /// </summary>
        /// <param name="appId"></param>
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public void App(int? appId) => Real.App(appId);

        /// <summary>
        /// used to be GET Module/GetSelectableApps
        /// </summary>
        /// <param name="apps"></param>
        /// <returns></returns>
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public IEnumerable<AppUiInfo> Apps(string apps = null) => Real.Apps(apps);

        #endregion

        #region Types

        /// <inheritdoc />
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public IEnumerable<ContentTypeUiInfo> ContentTypes() => Real.ContentTypes();

        #endregion

        #region Templates

        /// <summary>
        /// used to be GET Module/GetSelectableTemplates
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public IEnumerable<TemplateUiInfo> Templates() => Real.Templates();

        /// <summary>
        /// Used in InPage.js
        /// used to be GET Module/SaveTemplateId
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="forceCreateContentGroup"></param>
        /// <returns></returns>
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        //[Authorize(Roles = RoleNames.Registered)]
        [Authorize(Roles = RoleNames.Admin)]
        // TODO: 2DM please check permissions
        public Guid? Template(int templateId, bool forceCreateContentGroup) => Real.Template(templateId, forceCreateContentGroup);

        #endregion

        /// <inheritdoc />
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public AjaxRenderDto Render(int templateId, string lang) => Real.Set(OqtConstants.UiRoot).Render(templateId, lang);

        /// <inheritdoc />
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public bool Publish(string part, int index) => Real.Publish(part, index);

    }
}