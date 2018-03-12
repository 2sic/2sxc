using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DotNetNuke.Entities.Host;
using DotNetNuke.Security;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.WebApi;
using System.Configuration;
using System.Web.Configuration;
using System.Web.Http.Controllers;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Apps.Interfaces;
using ToSic.SexyContent.Environment.Dnn7;
using Factory = ToSic.Eav.Factory;

namespace ToSic.SexyContent.Adam
{
    /// <summary>
    /// Direct access to app-content items, simple manipulations etc.
    /// Should check for security at each standard call - to see if the current user may do this
    /// Then we can reduce security access level to anonymous, because each method will do the security check
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
   public class AdamController: SxcApiController
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("AdamCont");
        }

        public AdamBrowseContext AdamBrowseContext;

        private void PrepCore(Guid entityGuid, string fieldName, bool usePortalRoot)
        {
            AdamBrowseContext = new AdamBrowseContext(SxcContext, App, new DnnTenant(Dnn.Portal), entityGuid, fieldName, usePortalRoot);
        }

        public int MaxFileSizeKb 
            => (ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection)?.MaxRequestLength ?? 1; // if not specified, go to very low value, but not 0, as that could be infinite...


        [HttpPost]
        [HttpPut]
        public UploadResult Upload(string contentType, Guid guid, string field, [FromUri] string subFolder = "", bool usePortalRoot = false) 
            => UploadOne(contentType, guid, field, subFolder, usePortalRoot);

        private UploadResult UploadOne(string contentTypeName, Guid guid, string field, string subFolder, bool usePortalRoot)
        {
            Log.Add($"upload one a:{App?.AppId}, i:{guid}, field:{field}, subfold:{subFolder}, useRoot:{usePortalRoot}");
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            ExplicitlyRecheckEditPermissions();
            PrepCore(guid, field, usePortalRoot);

            // Get the content-type definition
            var cache = App.Data.Cache;
            var contentType = cache.GetContentType(contentTypeName);
            var fieldDef = contentType[field];

            // check if this field exists and is actually a file-field or a string (wysiwyg) field
            if (fieldDef == null || !(fieldDef.Type != "Hyperlink" || fieldDef.Type != "String"))
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Requested field '" + field + "' type doesn't allow upload"));
            Log.Add($"field type:{fieldDef.Type}");
            // check for special extensions

            try
            {
                var folder = AdamBrowseContext.Folder();
                if(!string.IsNullOrEmpty(subFolder)) 
                    folder = AdamBrowseContext.Folder(subFolder, false);
                var filesCollection = HttpContext.Current.Request.Files;
                Log.Add($"folder: {folder}, file⋮{filesCollection.Count}");
                if (filesCollection.Count > 0)
                {
                    var originalFile = filesCollection[0];

                    #region check content-type extensions...

                    // Check file size and extension
                    if(!IsAllowedDnnExtension(originalFile.FileName))
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    // todo: check metadata of the FieldDef to see if still allowed

                    #endregion

                    if (originalFile.ContentLength > (1024 * MaxFileSizeKb))
                        return new UploadResult { Success = false, Error = App.Resources.UploadFileSizeLimitExceeded };

                    // remove forbidden / troubling file name characters
                    var fileName = originalFile.FileName
                        .Replace("%", "per")
                        .Replace("#", "hash");

                    if (fileName != originalFile.FileName)
                        Log.Add($"cleaned file name from'{originalFile.FileName}' to '{fileName}'");

                    var dnnFolder = FolderManager.Instance.GetFolder(folder.Id);

                    // Make sure the image does not exist yet (change file name)
                    for (int i = 1; FileManager.Instance.FileExists(dnnFolder, Path.GetFileName(fileName)); i++)
                        fileName = Path.GetFileNameWithoutExtension(fileName)
                                   + "-" + i +
                                   Path.GetExtension(fileName);

                    // Everything is ok, add file
                    var dnnFile = FileManager.Instance.AddFile(dnnFolder, Path.GetFileName(fileName), originalFile.InputStream);

                    return new UploadResult
                    {
                        Success = true,
                        Error = "",
                        Name = Path.GetFileName(fileName),
                        Id = dnnFile.FileId,
                        Path = dnnFile.RelativePath,
                        Type = Classification.TypeName(dnnFile.Extension)
                    };
                }
                Log.Add("upload one complete");
                return new UploadResult { Success = false, Error = "No image was uploaded." };
            }
            catch (Exception e)
            {
                return new UploadResult { Success = false, Error = e.Message };
            }

            #region experiment with asyc - not supported by current version of .net framework
            //string root = HttpContext.Current.Server.MapPath("~/App_Data");
            //var provider = new MultipartFormDataStreamProvider(root);

            //try
            //{
            //    // Read the form data.
            //    // todo: try to get this async if possible!
            //    var task = Request.Content.ReadAsMultipartAsync(provider);
            //    task.Wait();

            //    //await Request.Content.ReadAsMultipartAsync(provider);
            //    //await TaskEx.Run(async () => await Request.Content.ReadAsMultipartAsync(provider));

            //    // This illustrates how to get the file names.
            //    foreach (MultipartFileData file in provider.FileData)
            //    {
            //        // Check file size and extension
            //        var extension = Path.GetExtension(file.Headers.ContentDisposition.FileName).ToLower().Replace(".", "");
            //        if (!allowedExtensions.Contains(extension))
            //        {
            //            throw new HttpResponseException(HttpStatusCode.Forbidden);
            //            // return new UploadResult { Success = false, Error = App.Resources.UploadExtensionNotAllowed };
            //        }
            //        // todo: save it
            //        // Trace.WriteLine(file.Headers.ContentDisposition.FileName);
            //        // Trace.WriteLine("Server file path: " + file.LocalFileName);
            //    }
            //    return Request.CreateResponse(HttpStatusCode.OK);
            //}
            //catch (System.Exception e)
            //{
            //    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            //}
            #endregion
        }


        #region adam-file manager

        [HttpGet]
        public IEnumerable<AdamItem> Items(Guid guid, string field, string subfolder, bool usePortalRoot = false)
        {
            Log.Add($"adam items a:{App?.AppId}, i:{guid}, field:{field}, subfold:{subfolder}, useRoot:{usePortalRoot}");
            ExplicitlyRecheckEditPermissions();
            PrepCore(guid, field, usePortalRoot);
            var folderManager = FolderManager.Instance;

            // get root and at the same time auto-create the core folder in case it's missing (important)
            AdamBrowseContext.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            var currentAdam = AdamBrowseContext.Folder(subfolder, false);
            var currentDnn = folderManager.GetFolder(currentAdam.Id);

            var subfolders =  folderManager.GetFolders(currentDnn);
            var files = folderManager.GetFiles(currentDnn);

            var adamFolders =
                subfolders.Where(s => s.FolderID != currentDnn.FolderID)
                    .Select(f => new AdamItem(f) {MetadataId = AdamBrowseContext.GetMetadataId(f.FolderID, true)})
                    .ToList();
            var adamFiles = files
                .Select(f => new AdamItem(f) {MetadataId = AdamBrowseContext.GetMetadataId(f.FileId, false), Type = Classification.TypeName(f.Extension)})
                .ToList();

            var all = adamFolders.Concat(adamFiles).ToList();

            Log.Add($"items complete - will return fld⋮{adamFolders.Count}, files⋮{adamFiles.Count} tot⋮{all.Count}");
            return all;
        }

        /// <summary>
        /// Explicitly re-check dnn security
        /// The user needs edit (not read) permissions even in read-scenarios
        /// because there is no other reason to access these assets
        /// </summary>
        private void ExplicitlyRecheckEditPermissions()
        {
            Log.Add("explicitly recheck permissions, will throw if not ok");
            var userMayEdit = Factory.Resolve<IPermissions>().UserMayEditContent(SxcContext.InstanceInfo);

            if (!userMayEdit)
                throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        [HttpPost]
        public IEnumerable<AdamItem> Folder(Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot)
        {
            Log.Add($"get folders for a:{App?.AppId}, i:{guid}, field:{field}, subfld:{subfolder}, new:{newFolder}, useRoot:{usePortalRoot}");
            ExplicitlyRecheckEditPermissions();
            PrepCore(guid, field, usePortalRoot);

            // get root and at the same time auto-create the core folder in case it's missing (important)
            AdamBrowseContext.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            AdamBrowseContext.Folder(subfolder, false);

            // now access the subfolder, creating it if missing (which is what we want
            AdamBrowseContext.Folder(subfolder + "/" + newFolder, true);

            return Items(guid, field, subfolder, usePortalRoot);
        }

        [HttpGet]
        public bool Delete(Guid guid, string field, string subfolder, bool isFolder, int id, bool usePortalRoot)
        {
            Log.Add($"delete from a:{App?.AppId}, i:{guid}, field:{field}, file:{id}, subf:{subfolder}, isFld:{isFolder}, useRoot:{usePortalRoot}");
            ExplicitlyRecheckEditPermissions();
            PrepCore(guid, field, usePortalRoot);

            // try to see if we can get into the subfolder - will throw error if missing
            var current = AdamBrowseContext.Folder(subfolder, false);
            
            var folderManager = FolderManager.Instance;
            var fileManager = FileManager.Instance;

            if (isFolder)
            {
                var fld = folderManager.GetFolder(id);
                if (fld.ParentID == current.Id)
                    folderManager.DeleteFolder(id);
                else
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) {ReasonPhrase = "can't delete folder - not found in folder"});
            }
            else
            {
                var file = fileManager.GetFile(id);
                if (file.FolderId == current.Id)
                    fileManager.DeleteFile(file);
                else
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) {ReasonPhrase = "can't delete file - not found in folder"});
            }

            Log.Add("delete complete");
            return true;
        }

        [HttpGet]
        public bool Rename(Guid guid, string field, string subfolder, bool isFolder, int id, string newName, bool usePortalRoot)
        {
            Log.Add($"rename a:{App?.AppId}, i:{guid}, field:{field}, subf:{subfolder}, isfld:{isFolder}, new:{newName}, useRoot:{usePortalRoot}");
            ExplicitlyRecheckEditPermissions();
            PrepCore(guid, field, usePortalRoot);

            // try to see if we can get into the subfolder - will throw error if missing
            var current = AdamBrowseContext.Folder(subfolder, false);

            var folderManager = FolderManager.Instance;
            var fileManager = FileManager.Instance;

            if (isFolder)
            {
                var fld = folderManager.GetFolder(id);
                if (fld.ParentID == current.Id)
                    folderManager.RenameFolder(fld, newName);
                else
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "can't rename folder - not found in folder" });
            }
            else
            {
                var file = fileManager.GetFile(id);
                if (file.Extension != newName.Split('.').Last())
                    newName += "." + file.Extension;

                if (file.FolderId != current.Id)
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "can't rename file - not found in folder" });

                fileManager.RenameFile(file, newName);
            }

            Log.Add("rename complete");
            return true;
        }



        #endregion


        #region Helper to check extension based on DNN settings
        // mostly a copy from https://github.com/dnnsoftware/Dnn.Platform/blob/115ae75da6b152f77ad36312eb76327cdc55edd7/DNN%20Platform/Modules/Journal/FileUploadController.cs#L72
        private static bool IsAllowedDnnExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            //regex matches a dot followed by 1 or more chars followed by a semi-colon
            //regex is meant to block files like "foo.asp;.png" which can take advantage
            //of a vulnerability in IIS6 which treasts such files as .asp, not .png
            return !string.IsNullOrEmpty(extension)
                   && Host.AllowedExtensionWhitelist.IsAllowedExtension(extension.ToLower());
        }
        #endregion

    }
}