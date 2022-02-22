using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi.Adam;

// #todo: security checks on APIs still completely missing
// #todo: upload not implemented yet

namespace IntegrationSamples.SxcEdit01.Controllers
{
    /// <summary>
    /// Direct access to app-content items, simple manipulations etc.
    /// Should check for security at each standard call - to see if the current user may do this
    /// Then we can reduce security access level to anonymous, because each method will do the security check
    /// </summary>
    [ApiController]
    [Route(WebApiConstants.DefaultRouteRoot + "/app/auto" + "/data/{contentType}/{guid}/{field}/")]
    public class AdamController : IntStatefulControllerBase, IAdamController<string>
    {
        protected override string HistoryLogName => "Api.Adam";

        public AdamController(AdamControllerReal<string> realController) => RealController = realController.Init(Log);
        public AdamControllerReal<string> RealController;

        
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
        public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, string id, bool usePortalRoot)
            => RealController.Delete(appId, contentType, guid, field, subfolder, isFolder, id, usePortalRoot);

        [HttpGet("rename")]
        public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, string id, string newName, bool usePortalRoot)
            => RealController.Rename(appId, contentType, guid, field, subfolder, isFolder, id, newName, usePortalRoot);

    }
}