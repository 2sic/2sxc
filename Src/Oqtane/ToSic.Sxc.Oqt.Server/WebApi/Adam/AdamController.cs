using Microsoft.AspNetCore.Mvc;
using System;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Sxc.Backend.Adam.AdamControllerReal<int>;

namespace ToSic.Sxc.Oqt.Server.WebApi.Adam;

/// <summary>
/// Direct access to app-content items, simple manipulations etc.
/// Should check for security at each standard call - to see if the current user may do this
/// Then we can reduce security access level to anonymous, because each method will do the security check
/// </summary>
//[SupportedModules("2sxc,2sxc-app")]
//[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)] // use view, all methods must re-check permissions
//[Authorize(Roles = RoleNames.Everyone)] commented because of http403 issue
[ValidateAntiForgeryToken]

// Release routes
[Route(OqtWebApiConstants.AppRootNoLanguage + "/{appName}/content/{contentType}/{guid:guid}/{field}")]
[Route(OqtWebApiConstants.AppRootPathOrLang + "/{appName}/content/{contentType}/{guid:guid}/{field}")]
[Route(OqtWebApiConstants.AppRootPathAndLang + "/{appName}/content/{contentType}/{guid:guid}/{field}")]
[Route(OqtWebApiConstants.AppRootNoLanguage + "/{appName}/data/{contentType}/{guid:guid}/{field}")] // new, v13
[Route(OqtWebApiConstants.AppRootPathOrLang + "/{appName}/data/{contentType}/{guid:guid}/{field}")] // new, v13
[Route(OqtWebApiConstants.AppRootPathAndLang + "/{appName}/data/{contentType}/{guid:guid}/{field}")] // new, v13
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AdamController() : OqtStatefulControllerBase("Adam"), IAdamController<int>
{
    private RealController Real => GetService<RealController>();

    // Note: #AdamItemDto - as of now, we must use object because System.Io.Text.Json will otherwise not convert the object correctly :(

    [HttpPost]
    [HttpPut]
    public /*AdamItemDto*/object Upload(int appId, string contentType, Guid guid, string field, string subFolder = "", bool usePortalRoot = false) 
        => Real.Upload(new(Request), appId, contentType, guid, field, subFolder, usePortalRoot);

    [HttpGet("items")]
    public IEnumerable</*AdamItemDto*/object> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false)
        => Real.Items(appId, contentType, guid, field, subfolder, usePortalRoot)
            // Fix bug with .net 7 so that we really return a fresh IEnumerable and not the initial list
            // Otherwise System.Text.Json sees the List<AdamItemDto> and will not convert additional properties on the objects
            .Select(e => e); 

    [HttpPost("folder")]
    public IEnumerable</*AdamItemDto*/object> Folder(int appId, string contentType, Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot)
        => Real.Folder(appId, contentType, guid, field, subfolder, newFolder, usePortalRoot)
            // Fix bug with .net 7 so that we really return a fresh IEnumerable and not the initial list
            // Otherwise System.Text.Json sees the List<AdamItemDto> and will not convert additional properties on the objects
            .Select(e => e);

    [HttpGet("delete")]
    public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, bool usePortalRoot)
        => Real.Delete(appId, contentType, guid, field, subfolder, isFolder, id, usePortalRoot );

    [HttpGet("rename")]
    public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, string newName, bool usePortalRoot)
        => Real.Rename(appId, contentType, guid, field, subfolder, isFolder, id, newName, usePortalRoot);

}