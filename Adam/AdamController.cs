using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.WebApi;

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
        public Core Core;

        private void PrepCore(Guid entityGuid, string fieldName)
        {
            Core = new Core(App, Dnn, entityGuid, fieldName);
        }

        // todo: centralize once it works
        // todo:idea that it would auto-take a setting from app-settings if it exists :)
        private string AdamAllowedExtensions =
            "jpg,jpeg,jpe,gif,bmp,png,doc,docx,xls,xlsx,ppt,pptx,pdf,txt,xml,xsl,xsd,css,zip,ico,avi,mpg,mpeg,mp3,wmv,mov,wav,ico,vcf";
        public const int MaxFileSizeMb = 10;


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

            // check if this field exists and is actually a file-field
            if (fieldDef == null || fieldDef.Type != "Hyperlink")
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Requested field '" + field + "' type doesn't allow upload"));// { HttpStatusCode = HttpStatusCode.BadRequest });

            // check for special extensions

            try
            {
                var folder = Core.Folder();
                if(!string.IsNullOrEmpty(subFolder)) 
                    folder = Core.Folder(subFolder, false);
                var filesCollection = HttpContext.Current.Request.Files;
                if (filesCollection.Count > 0)
                {
                    var originalFile = filesCollection[0];

                    // todo: check content-type extensions...

                    // Check file size and extension
                    var extension = Path.GetExtension(originalFile.FileName).ToLower().Replace(".", "");
                    if (!AdamAllowedExtensions.Contains(extension))
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    if (originalFile.ContentLength > (1024 * 1024 * MaxFileSizeMb))
                        return new UploadResult { Success = false, Error = App.Resources.UploadFileSizeLimitExceeded };

                    var fileName = originalFile.FileName
                        .Replace("%", "per");

                    // Make sure the image does not exist yet (change file name)
                    for (int i = 1; FileManager.Instance.FileExists(folder, Path.GetFileName(fileName)); i++)
                    {
                        fileName = Path.GetFileNameWithoutExtension(originalFile.FileName) + "-" + i +
                                    Path.GetExtension(originalFile.FileName);
                    }

                    // Everything is ok, add file
                    var dnnFile = FileManager.Instance.AddFile(folder, Path.GetFileName(fileName), originalFile.InputStream);

                    return new UploadResult { Success = true, Error = "", Filename = Path.GetFileName(fileName), FileId = dnnFile.FileId, FullPath = dnnFile.RelativePath };
                }

                return new UploadResult { Success = false, Error = "No image was uploaded." };
            }
            catch (Exception e)
            {
                return new UploadResult { Success = false, Error = e.Message };
            }


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
        }

        public class UploadResult
        {
            public bool Success { get; set; }
            public string Error { get; set; }
            public string Filename { get; set; }
            public int FileId { get; set; }
            public string FullPath { get; set; }
        }

        #region adam-file manager

        [HttpGet]
        public IEnumerable<AdamItem> Items(Guid guid, string field, string subfolder)
        {
            ExplicitlyRecheckEditPermissions();
            PrepCore(guid, field);
            var folderManager = FolderManager.Instance;

            // get root and at the same time auto-create the core folder in case it's missing (important)
            Core.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            var current = Core.Folder(subfolder, false);

            var subfolders = folderManager.GetFolders(current);
            var files = folderManager.GetFiles(current);

            var adamFolders =
                subfolders.Where(s => s.FolderID != current.FolderID)
                    .Select(f => new AdamItem(f) {MetadataId = Core.GetMetadataId(f.FolderID, true)});
            var adamFiles = files
                .Select(f => new AdamItem(f) {MetadataId = Core.GetMetadataId(f.FileId, false)});

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
            if (!DotNetNuke.Security.Permissions.ModulePermissionController.CanEditModuleContent(Dnn.Module))
                throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        [HttpPost]
        public IEnumerable<AdamItem> Folder(Guid guid, string field, string subfolder, string newFolder)
        {
            ExplicitlyRecheckEditPermissions();
            PrepCore(guid, field);

            // get root and at the same time auto-create the core folder in case it's missing (important)
            Core.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            Core.Folder(subfolder, false);

            // now access the subfolder, creating it if missing (which is what we want
            Core.Folder(subfolder + "/" + newFolder, true);

            return Items(guid, field, subfolder);
        }

        [HttpGet]
        public bool Delete(Guid guid, string field, string subfolder, bool isFolder, int id)
        {
            ExplicitlyRecheckEditPermissions();
            PrepCore(guid, field);

            // try to see if we can get into the subfolder - will throw error if missing
            var current = Core.Folder(subfolder, false);
            
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


    }
}