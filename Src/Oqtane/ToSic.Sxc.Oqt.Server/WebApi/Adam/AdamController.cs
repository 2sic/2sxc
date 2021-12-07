using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Errors;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Adam;
using ToSic.Sxc.WebApi.PublicApi;

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
    //[Route(WebApiConstants.WebApiStateRoot + "/app-content/{contentType}/{guid:guid}/{field}/[action]")]
    [Route(WebApiConstants.WebApiStateRoot + "/app-content/{contentType}/{guid:guid}/{field}")]

    public class AdamController : OqtStatefulControllerBase, IAdamController<int>
    {
        private readonly Lazy<AdamTransUpload<int, int>> _adamUpload;
        private readonly Lazy<AdamTransGetItems<int, int>> _adamItems;
        private readonly Lazy<AdamTransFolder<int, int>> _adamFolders;
        private readonly Lazy<AdamTransDelete<int, int>> _adamDelete;
        private readonly Lazy<AdamTransRename<int, int>> _adamRename;

        #region Constructor / DI

        public AdamController(
            Lazy<AdamTransUpload<int, int>> adamUpload,
            Lazy<AdamTransGetItems<int, int>> adamItems,
            Lazy<AdamTransFolder<int, int>> adamFolders,
            Lazy<AdamTransDelete<int, int>> adamDelete,
            Lazy<AdamTransRename<int, int>> adamRename)
        {
            _adamUpload = adamUpload;
            _adamItems = adamItems;
            _adamFolders = adamFolders;
            _adamDelete = adamDelete;
            _adamRename = adamRename;
        }

        #endregion

        protected override string HistoryLogName => "Api.Adam";

        [HttpPost]
        [HttpPut]
        public UploadResultDto Upload(int appId, string contentType, Guid guid, string field, string subFolder = "", bool usePortalRoot = false)
        {
            // wrap all of it in try/catch, to reformat error in better way for js to tell the user
            try
            {
                // Check if the request contains multipart/form-data.
                if (Request.Form.Files.Count < 1) // ..Content.IsMimeMultipartContent())
                    return new UploadResultDto
                    {
                        Success = false,
                        Error = "doesn't look like a file-upload"
                    };

                var filesCollection = Request.Form.Files;
                if (filesCollection.Count <= 0)
                {
                    Log.Add("Error, no files");
                    return new UploadResultDto { Success = false, Error = "No file was uploaded." };
                }

                var originalFile = filesCollection[0];
                var stream = originalFile.OpenReadStream();
                var fileName = originalFile.FileName;
                var uploader = _adamUpload.Value.Init(appId, contentType, guid, field, usePortalRoot, Log);
                return uploader.UploadOne(stream, subFolder, fileName);
            }
            catch (HttpExceptionAbstraction he)
            {
                return new UploadResultDto { Success = false, Error = he.Message };
            }
            catch (Exception e)
            {
                return new UploadResultDto { Success = false, Error = e.Message };
            }
        }


        #region adam-file manager

        // test method to provide a public API for accessing adam items easily
        // todo #Oqtane not really implemented, not sure if needed
        //[HttpGet]
        //public IEnumerable<AdamItemDto> ItemsWithAppIdFromContext(string contentType, Guid guid, string field, string folder = "")
        //{
        //    // if app-path specified, use that app, otherwise use from context
        //    var appId = GetBlock().AppId;
        //    return Items(appId, contentType, guid, field, folder);
        //}

        [HttpGet("items")]
        public IEnumerable<AdamItemDto> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false)
        {
            var callLog = Log.Call<IEnumerable<AdamItemDto>>($"adam items a:{appId}, i:{guid}, field:{field}, subfolder:{subfolder}, useRoot:{usePortalRoot}");
            var results = _adamItems.Value
                .Init(appId, contentType, guid, field, usePortalRoot, Log)
                .ItemsInField(subfolder);
            return callLog("ok",  results);
        }

        [HttpPost("folder")]
        public IEnumerable<AdamItemDto> Folder(int appId, string contentType, Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot)
            => _adamFolders.Value
                .Init(appId, contentType, guid, field, usePortalRoot, Log)
                .Folder(subfolder, newFolder);

        [HttpGet("delete")]
        public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, bool usePortalRoot)
            => _adamDelete.Value
                .Init(appId, contentType, guid, field, usePortalRoot, Log)
                .Delete(subfolder, isFolder, id, id);

        [HttpGet("rename")]
        public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, string newName, bool usePortalRoot)
            => _adamRename.Value
                .Init(appId, contentType, guid, field, usePortalRoot, Log)
                .Rename(subfolder, isFolder, id, id, newName);

        #endregion

    }
}