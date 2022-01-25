using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.Dnn.WebApi
{
    /// <summary>
    /// Direct access to app-content items, simple manipulations etc.
    /// Should check for security at each standard call - to see if the current user may do this
    /// Then we can reduce security access level to anonymous, because each method will do the security check
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]    // use view, all methods must re-check permissions
    [ValidateAntiForgeryToken]
    public class AdamController : SxcApiControllerBase, IAdamController<int>
    {
        protected override string HistoryLogName => "Api.Adam";

        [HttpPost]
        [HttpPut]
        public UploadResultDto Upload(int appId, string contentType, Guid guid, string field, [FromUri] string subFolder = "", bool usePortalRoot = false)
        {
            // wrap all of it in try/catch, to reformat error in better way for js to tell the user
            try
            {
                // Check if the request contains multipart/form-data.
                if (!Request.Content.IsMimeMultipartContent())
                    return new UploadResultDto
                    {
                        Success = false,
                        Error = "doesn't look like a file-upload"
                    };

                var filesCollection = HttpContext.Current.Request.Files;
                if (filesCollection.Count <= 0)
                {
                    Log.Add("Error, no files");
                    return new UploadResultDto { Success = false, Error = "No file was uploaded." };
                }

                var originalFile = filesCollection[0];
                var stream = originalFile.InputStream;
                var fileName = originalFile.FileName;
                var uploader = GetService<AdamTransUpload<int, int>>().Init(appId, contentType, guid, field, usePortalRoot, Log);
                return uploader.UploadOne(stream, subFolder, fileName);
            }
            catch (HttpExceptionAbstraction he)
            {
                // Our abstraction places an extra message in the value, not sure if this is right, but that's how it is. 
                return new UploadResultDto { Success = false, Error = he.Response.ReasonPhrase + "\n" + he.Value };
            }
            catch (Exception e)
            {
                return new UploadResultDto { Success = false, Error = e.Message + "\n" + e.Message };
            }
        }


        #region adam-file manager

        // test method to provide a public API for accessing adam items easily
        // not sure if it is ever used
        [HttpGet]
        public IEnumerable<AdamItemDto> Items(string contentType, Guid guid, string field, string folder = "")
        {
            // if app-path specified, use that app, otherwise use from context
            const int AutoDetect = -1;
            return Items(AutoDetect, contentType, guid, field, folder);
        }

        [HttpGet]
        public IEnumerable<AdamItemDto> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false)
        {
            var callLog = Log.Call<IEnumerable<AdamItemDto>>($"adam items a:{appId}, i:{guid}, field:{field}, subfolder:{subfolder}, useRoot:{usePortalRoot}");
            var results = GetService<AdamTransGetItems<int, int>>()
                .Init(appId, contentType, guid, field, usePortalRoot, Log)
                .ItemsInField(subfolder);
            return callLog("ok",  results);
        }

        [HttpPost]
        public IEnumerable<AdamItemDto> Folder(int appId, string contentType, Guid guid, string field, string subfolder,
            string newFolder, bool usePortalRoot)
            => GetService<AdamTransFolder<int, int>>()
                .Init(appId, contentType, guid, field, usePortalRoot, Log)
                .Folder(subfolder, newFolder);

        [HttpGet]
        public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder,
            int id, bool usePortalRoot)
            => GetService<AdamTransDelete<int, int>>()
                .Init(appId, contentType, guid, field, usePortalRoot, Log)
                .Delete(subfolder, isFolder, id, id);

        [HttpGet]
        public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder,
            int id, string newName, bool usePortalRoot)
            => GetService<AdamTransRename<int, int>>()
                .Init(appId, contentType, guid, field, usePortalRoot, Log)
                .Rename(subfolder, isFolder, id, id, newName);

        #endregion
    }
}