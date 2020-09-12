using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.WebApi.Adam;
using ToSic.Sxc.WebApi.Errors;

// #todo: security checks on APIs still completely missing
// #todo: upload not implemented yet

namespace ToSic.Sxc.Mvc.WebApi.Adam
{
    /// <summary>
    /// Direct access to app-content items, simple manipulations etc.
    /// Should check for security at each standard call - to see if the current user may do this
    /// Then we can reduce security access level to anonymous, because each method will do the security check
    /// </summary>
    [ApiController]
    [Route(WebApiConstants.WebApiRoot + "/app-content/{contentType}/{guid}/{field}/")]
    public class AdamController : SxcStatefullControllerBase
    {
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
                var uploader = new AdamTransUpload<int, int>().Init(GetBlock(), appId, contentType, guid, field, usePortalRoot, Log);
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
        [HttpGet]
        public IEnumerable<AdamItemDto> ItemsWithAppIdFromContext(string contentType, Guid guid, string field, string folder = "")
        {
            // if app-path specified, use that app, otherwise use from context
            var appId = GetBlock().AppId;
            return Items(appId, contentType, guid, field, folder);
        }

        [HttpGet("items")]
        public IEnumerable<AdamItemDto> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false)
        {
            var callLog = Log.Call<IEnumerable<AdamItemDto>>($"adam items a:{appId}, i:{guid}, field:{field}, subfolder:{subfolder}, useRoot:{usePortalRoot}");
            var results = new AdamTransGetItems<string, string>()
                .Init(GetBlock(), appId, contentType, guid, field, usePortalRoot, Log)
                .ItemsInField(subfolder);
            return callLog("ok",  results);
        }

        [HttpPost("folder")]
        public IEnumerable<AdamItemDto> Folder(int appId, string contentType, Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot) 
            => new AdamTransFolder<string, string>()
                .Init(GetBlock(), appId, contentType, guid, field, usePortalRoot, Log)
                .Folder(subfolder, newFolder);

        [HttpGet("delete")]
        public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, bool usePortalRoot) 
            => new AdamTransDelete<string, string>()
                .Init(GetBlock(), appId, contentType, guid, field, usePortalRoot, Log)
                .Delete(subfolder, isFolder, id.ToString(), id.ToString());

        [HttpGet("rename")]
        public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, string newName, bool usePortalRoot) 
            => new AdamTransRename<string, string>()
                .Init(GetBlock(), appId, contentType, guid, field, usePortalRoot, Log)
                .Rename(subfolder, isFolder, id.ToString(), id.ToString(), newName);

        #endregion

    }
}