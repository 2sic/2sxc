using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.Oqt.Server.WebApi.Adam
{
    /// <summary>
    /// Direct access to app-content items, simple manipulations etc.
    /// Should check for security at each standard call - to see if the current user may do this
    /// Then we can reduce security access level to anonymous, because each method will do the security check
    /// </summary>
    //[SupportedModules("2sxc,2sxc-app")]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)] // use view, all methods must re-check permissions
    //[Authorize(Roles = RoleNames.Everyone)] commented because of http403 issue
    // TODO: 2DM please check permissions
    [ValidateAntiForgeryToken]

    // Release routes
    [Route(WebApiConstants.AppRoot + "/{appName}/content/{contentType}/{guid:guid}/{field}")]
    [Route(WebApiConstants.AppRoot2 + "/{appName}/content/{contentType}/{guid:guid}/{field}")]
    [Route(WebApiConstants.AppRoot3 + "/{appName}/content/{contentType}/{guid:guid}/{field}")]
    [Route(WebApiConstants.AppRoot + "/{appName}/data/{contentType}/{guid:guid}/{field}")] // new, v13
    [Route(WebApiConstants.AppRoot2 + "/{appName}/data/{contentType}/{guid:guid}/{field}")] // new, v13
    [Route(WebApiConstants.AppRoot3 + "/{appName}/data/{contentType}/{guid:guid}/{field}")] // new, v13
    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/app-content/{contentType}/{guid:guid}/{field}")]

    public class AdamController : OqtStatefulControllerBase, IAdamController<int>
    {
        #region Constructor / DI

        public AdamController(AdamControllerReal<int> realController) => RealController = realController.Init(Log);
        public AdamControllerReal<int> RealController;

        #endregion

        protected override string HistoryLogName => "Api.Adam";

        [HttpPost]
        [HttpPut]
        public UploadResultDto Upload(int appId, string contentType, Guid guid, string field, string subFolder = "", bool usePortalRoot = false) 
            => RealController.Upload(new HttpUploadedFile(Request), appId, contentType, guid, field, subFolder, usePortalRoot);

        [HttpGet("items")]
        public IEnumerable<AdamItemDto> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false)
            => RealController.Items(appId, contentType, guid, field, subfolder, usePortalRoot);

        [HttpPost("folder")]
        public IEnumerable<AdamItemDto> Folder(int appId, string contentType, Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot)
            => RealController.Folder(appId, contentType, guid, field, subfolder, newFolder, usePortalRoot);

        [HttpGet("delete")]
        public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, bool usePortalRoot)
            => RealController.Delete(appId, contentType, guid, field, subfolder, isFolder, id, usePortalRoot );

        [HttpGet("rename")]
        public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, string newName, bool usePortalRoot)
            => RealController.Rename(appId, contentType, guid, field, subfolder, isFolder, id, newName, usePortalRoot);

    }
}