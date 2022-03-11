using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.WebApi.Adam;

// #todo: security checks on APIs still completely missing

namespace IntegrationSamples.SxcEdit01.Controllers
{
    /// <summary>
    /// Direct access to app-content items, simple manipulations etc.
    /// Should check for security at each standard call - to see if the current user may do this
    /// Then we can reduce security access level to anonymous, because each method will do the security check
    /// </summary>
    [ApiController]
    [Route(IntegrationConstants.DefaultRouteRoot + AppRoots.AppAutoData + "/" + ValueTokens.SetTypeGuidField)]
    public class AdamController : IntControllerBase<AdamControllerReal<string>>, IAdamController<string>
    {
        // IMPORTANT: Uses the Proxy/Real concept - see https://r.2sxc.org/proxy-controllers

        public AdamController() : base("Adam") { }

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
        public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, string id, bool usePortalRoot)
            => Real.Delete(appId, contentType, guid, field, subfolder, isFolder, id, usePortalRoot);

        [HttpGet("rename")]
        public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, string id, string newName, bool usePortalRoot)
            => Real.Rename(appId, contentType, guid, field, subfolder, isFolder, id, newName, usePortalRoot);

    }
}