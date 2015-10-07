using System;
using System.IO;
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
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
   public class AdamController: SxcApiController
    {
        // todo: centralize once it works
        // todo:idea that it would auto-take a setting from app-settings if it exists :)

        public const string AdamRootFolder = "adam/";
        public const string AdamAppRootFolder = "adam/[AppPath]/";
        public const string AdamFolderMask = "adam/[AppPath]/[Guid22]/[FieldName]/";
        private string AdamAllowedExtensions = "jpg,png,gif"; // todo: pdf, word, etc.
        public const int MaxFileSizeMb = 10;

        #region Initializers

        // private Eav.WebApi.WebApi _eavWebApi;

        //public AdamController()
        //{
        //    // Eav.Configuration.SetConnectionString("SiteSqlServer");
        //}

        //private void InitEavAndSerializer()
        //{
        //    // Improve the serializer so it's aware of the 2sxc-context (module, portal etc.)
        //    _eavWebApi = new Eav.WebApi.WebApi(App.AppId);
        //    ((Serializer)_eavWebApi.Serializer).Sxc = Sexy;
        //}

        #endregion


        //[HttpPost]
        //[HttpPut]
        //public dynamic Upload(int id, string field)
        //{
        //    // get the entity
        //    var ent = App.Data["Default"].List[id];
        //    return Upload(ent, field);
        //}

        //[HttpPost]
        [HttpPut]
        public UploadResult Upload(string contentTypeName, Guid guid, string field)
        {
            return UploadOne(contentTypeName, guid, field);
        }

        private UploadResult UploadOne(string contentTypeName, Guid guid, string field)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            // check dnn security
            if (!DotNetNuke.Security.Permissions.ModulePermissionController.CanEditModuleContent(Dnn.Module))
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            // return new UploadResult { Success = false, Error = App.Resources.UploadNoPermission };

            // Get the content-type definition
            var cache = App.Data.Cache;
            var contentType = cache.GetContentType(contentTypeName);
            var fieldDef = contentType[field];

            // check if this field exists and is actually a file-field
            if (fieldDef == null || fieldDef.Type != "Hyperlink")
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Requested field '" + field + "' type doesn't allow upload"));// { HttpStatusCode = HttpStatusCode.BadRequest });

            try
            {
                var folder = Folder(guid, field);
                var filesCollection = HttpContext.Current.Request.Files;
                if (filesCollection.Count > 0)
                {
                    var originalFile = filesCollection[0];

                    // Check file size and extension
                    var extension = Path.GetExtension(originalFile.FileName).ToLower().Replace(".", "");
                    if (!AdamAllowedExtensions.Contains(extension))
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    if (originalFile.ContentLength > (1024 * 1024 * MaxFileSizeMb))
                        return new UploadResult { Success = false, Error = App.Resources.UploadFileSizeLimitExceeded };

                    var fileName = originalFile.FileName;

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



        private IFolderInfo _folder;

        /// <summary>
        /// Get the folder specified in App.Settings (BasePath) combined with the module's ID
        /// Will create the folder if it does not exist
        /// </summary>
        public IFolderInfo Folder(Guid entityGuid, string fieldName)
        {

            if (_folder == null)
            {
                var folderManager = FolderManager.Instance;

                var basePath = AdamAppRootFolder.Replace("[AppPath]", App.Path);

                var path = AdamFolderMask.Replace("[AppPath]", App.Path)
                    .Replace("[Guid22]", GuidHelpers.Compress22(entityGuid))
                    .Replace("[FieldName]", fieldName);

                // Create base folder if not exists
                if (!folderManager.FolderExists(Dnn.Portal.PortalId, basePath))
                    folderManager.AddFolder(Dnn.Portal.PortalId, basePath);

                // Create images folder for this module if not exists
                if (!folderManager.FolderExists(Dnn.Portal.PortalId, path))
                    folderManager.AddFolder(Dnn.Portal.PortalId, path);

                _folder = folderManager.GetFolder(Dnn.Portal.PortalId, path);
            }


            return _folder;
        }

    }
}