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
    [Route(WebApiConstants.AppRootNoLanguage + "/{appName}/content/{contentType}/{guid:guid}/{field}")]
    [Route(WebApiConstants.AppRootPathOrLang + "/{appName}/content/{contentType}/{guid:guid}/{field}")]
    [Route(WebApiConstants.AppRootPathNdLang + "/{appName}/content/{contentType}/{guid:guid}/{field}")]
    [Route(WebApiConstants.AppRootNoLanguage + "/{appName}/data/{contentType}/{guid:guid}/{field}")] // new, v13
    [Route(WebApiConstants.AppRootPathOrLang + "/{appName}/data/{contentType}/{guid:guid}/{field}")] // new, v13
    [Route(WebApiConstants.AppRootPathNdLang + "/{appName}/data/{contentType}/{guid:guid}/{field}")] // new, v13

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/app-content/{contentType}/{guid:guid}/{field}")]

    public class AdamController : OqtStatefulControllerBase, IAdamController<int>
    {
        // IMPORTANT: Uses the Proxy/Real concept - see https://r.2sxc.org/proxy-controllers

        protected override string HistoryLogName => "Api.Adam";

        public AdamController(AdamControllerReal<int> realController) => Real = realController.Init(Log);
        public AdamControllerReal<int> Real;


        [HttpPost]
        [HttpPut]
        public UploadResultDto Upload(int appId, string contentType, Guid guid, string field, string subFolder = "", bool usePortalRoot = false) 
            => Real.Upload(new HttpUploadedFile(Request), appId, contentType, guid, field, subFolder, usePortalRoot);

        [HttpGet("items")]
        public IEnumerable<AdamItemDto> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false)
            => Real.Items(appId, contentType, guid, field, subfolder, usePortalRoot);

        [HttpPost("folder")]
        public IEnumerable<AdamItemDto> Folder(int appId, string contentType, Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot)
            => Real.Folder(appId, contentType, guid, field, subfolder, newFolder, usePortalRoot);

        [HttpGet("delete")]
        public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, bool usePortalRoot)
            => Real.Delete(appId, contentType, guid, field, subfolder, isFolder, id, usePortalRoot );

        [HttpGet("rename")]
        public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, string newName, bool usePortalRoot)
            => Real.Rename(appId, contentType, guid, field, subfolder, isFolder, id, newName, usePortalRoot);

    }
}