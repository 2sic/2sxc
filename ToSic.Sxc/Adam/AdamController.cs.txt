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

namespace ToSic.SexyContent.Adam
{
    /// <summary>
    /// Direct access to app-content items, simple manipulations etc.
    /// Should check for security at each standard call - to see if the current user may do this
    /// Then we can reduce security access level to anonymous, because each method will do the security check
    /// todo: security
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
   public class AdamController: SxcApiController
    {
        public EntityBase EntityBase;

        private void PrepCore(Guid entityGuid, string fieldName)
        {
            EntityBase = new EntityBase(SxcContext, App, Dnn.Portal, entityGuid, fieldName);
        }

        public int MaxFileSizeKb
        {
            get
            {
                return (ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection).MaxRequestLength;
            }
        }


        [HttpPost]
        [HttpPut]
        public UploadResult Upload(string contentType, Guid guid, string field, [FromUri] string subFolder = "")
        {
            return UploadOne(contentType, guid, field, subFolder);
        }

        private UploadResult UploadOne(string contentTypeName, Guid guid, string field, string subFolder)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            ExplicitlyRecheckEditPermissions();
            PrepCore(guid, field);

            // Get the content-type definition
            var cache = App.Data.Cache;
            var contentType = cache.GetContentType(contentTypeName);
            var fieldDef = contentType[field];

            // check if this field exists and is actually a file-field or a string (wysiwyg) field
            if (fieldDef == null || !(fieldDef.Type != "Hyperlink" || fieldDef.Type != "String"))
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Requested field '" + field + "' type doesn't allow upload"));// { HttpStatusCode = HttpStatusCode.BadRequest });

            // check for special extensions

            try
            {
                var folder = EntityBase.Folder();
                if(!string.IsNullOrEmpty(subFolder)) 
                    folder = EntityBase.Folder(subFolder, false);
                var filesCollection = HttpContext.Current.Request.Files;
                if (filesCollection.Count > 0)
                {
                    var originalFile = filesCollection[0];

                    #region check content-type extensions...

                    // Check file size and extension
                    //var extension = Path.GetExtension(originalFile.FileName).ToLower().Replace(".", "");
                    //if (!AdamAllowedExtensions.Contains(extension))
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

                    // Make sure the image does not exist yet (change file name)
                    for (int i = 1; FileManager.Instance.FileExists(folder, Path.GetFileName(fileName)); i++)
                        fileName = Path.GetFileNameWithoutExtension(fileName)
                                   + "-" + i +
                                   Path.GetExtension(fileName);

                    // Everything is ok, add file
                    var dnnFile = FileManager.Instance.AddFile(folder, Path.GetFileName(fileName), originalFile.InputStream);

                    return new UploadResult
                    {
                        Success = true,
                        Error = "",
                        Name = Path.GetFileName(fileName),
                        Id = dnnFile.FileId,
                        Path = dnnFile.RelativePath,
                        Type = EntityBase.TypeName(dnnFile.Extension)
                    };
                }

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
        public IEnumerable<AdamItem> Items(Guid guid, string field, string subfolder)
        {
            ExplicitlyRecheckEditPermissions();
            PrepCore(guid, field);
            var folderManager = FolderManager.Instance;

            // get root and at the same time auto-create the core folder in case it's missing (important)
            EntityBase.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            var current = EntityBase.Folder(subfolder, false);

            var subfolders = folderManager.GetFolders(current);
            var files = folderManager.GetFiles(current);

            var adamFolders =
                subfolders.Where(s => s.FolderID != current.FolderID)
                    .Select(f => new AdamItem(f) {MetadataId = EntityBase.GetMetadataId(f.FolderID, true)});
            var adamFiles = files
                .Select(f => new AdamItem(f) {MetadataId = EntityBase.GetMetadataId(f.FileId, false), Type = EntityBase.TypeName(f.Extension)});

            var all = adamFolders.Concat(adamFiles);

            return all;
        }

        /// <summary>
        /// Explicitly re-check dnn security
        /// The user needs edit (not read) permissions even in read-scenarios
        /// because there is no other reason to access these assets
        /// </summary>
        private void ExplicitlyRecheckEditPermissions()
        {
            // 2016-03-30 2rm: Changed code below to work with the Environment permission controller
            if(!SxcContext.Environment.Permissions.UserMayEditContent)
                throw new HttpResponseException(HttpStatusCode.Forbidden);

            //if (!DotNetNuke.Security.Permissions.ModulePermissionController.CanEditModuleContent(Dnn.Module))
            //    throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        [HttpPost]
        public IEnumerable<AdamItem> Folder(Guid guid, string field, string subfolder, string newFolder)
        {
            ExplicitlyRecheckEditPermissions();
            PrepCore(guid, field);

            // get root and at the same time auto-create the core folder in case it's missing (important)
            EntityBase.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            EntityBase.Folder(subfolder, false);

            // now access the subfolder, creating it if missing (which is what we want
            EntityBase.Folder(subfolder + "/" + newFolder, true);

            return Items(guid, field, subfolder);
        }

        [HttpGet]
        public bool Delete(Guid guid, string field, string subfolder, bool isFolder, int id)
        {
            ExplicitlyRecheckEditPermissions();
            PrepCore(guid, field);

            // try to see if we can get into the subfolder - will throw error if missing
            var current = EntityBase.Folder(subfolder, false);
            
            var folderManager = FolderManager.Instance;
            var fileManager = FileManager.Instance;

            if (isFolder)
            {
                var fld = folderManager.GetFolder(id);
                if (fld.ParentID == current.FolderID)
                    folderManager.DeleteFolder(id);
                else
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) {ReasonPhrase = "can't delete folder - not found in folder"});
            }
            else
            {
                var file = fileManager.GetFile(id);
                if (file.FolderId == current.FolderID)
                    fileManager.DeleteFile(file);
                else
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) {ReasonPhrase = "can't delete file - not found in folder"});
            }

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