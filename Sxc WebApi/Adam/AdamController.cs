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
using System.Configuration;
using System.Linq.Expressions;
using System.Web.Configuration;
using System.Web.Http.Controllers;
using JetBrains.Annotations;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Identity;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Razor.Helpers;
using ToSic.SexyContent.WebApi;
using ToSic.SexyContent.WebApi.Errors;
using ToSic.SexyContent.WebApi.Permissions;
using App = ToSic.SexyContent.App;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Adam.WebApi
{
    /// <summary>
    /// Direct access to app-content items, simple manipulations etc.
    /// Should check for security at each standard call - to see if the current user may do this
    /// Then we can reduce security access level to anonymous, because each method will do the security check
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]    // use view, all methods must re-check permissions
   public class AdamController: SxcApiControllerBase
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("Api.Adam");
        }

        internal ContainerBase ContainerContext;
        internal AdamAppContext AdamAppContext;

        internal IAttributeDefinition CurrentAttribute;

        private void PrepCore(App app,  Guid entityGuid, string fieldName, bool usePortalRoot)
        {
            var dnn = new DnnHelper(SxcInstance?.EnvInstance);

            var tenant = new DnnTenant(dnn.Portal);
            AdamAppContext = new AdamAppContext(tenant, app, SxcInstance);
            ContainerContext = usePortalRoot 
                ? new ContainerOfTenant(AdamAppContext) as ContainerBase
                : new ContainerOfField(AdamAppContext, entityGuid, fieldName);

            // try to find attribute definition - for later extra security checks
            // WIP
            if (usePortalRoot) return;
            var appRead = new AppRuntime(app.AppId, Log);
            var ent = appRead.Entities.Get(entityGuid);
            var type = ent?.Type;
            var attrDef = type?.Attributes.FirstOrDefault(a => a.Name == fieldName);
            CurrentAttribute = attrDef;
        }

        public int MaxFileSizeKb 
            => (ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection)?.MaxRequestLength ?? 1; // if not specified, go to very low value, but not 0, as that could be infinite...


        [HttpPost]
        [HttpPut]
        public UploadResult Upload(int appId, string contentType, Guid guid, string field, [FromUri] string subFolder = "", bool usePortalRoot = false) 
            => UploadOne(appId, contentType, guid, field, subFolder, usePortalRoot);

        private UploadResult UploadOne(int appId, string contentType, Guid guid, string field, string subFolder, bool usePortalRoot)
        {
            Log.Add($"upload one a:{appId}, i:{guid}, field:{field}, subfold:{subFolder}, useRoot:{usePortalRoot}");
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var permCheck = new AppAndPermissions(SxcInstance, appId, Log);

            permCheck.EnsureOrThrow(GrantSets.WriteSomething, contentType);
            var onlyInsideAdam = !UserMayWriteEverywhereOrThrowIfAttempted(usePortalRoot, permCheck.Permissions);

            PrepCore(permCheck.App, guid, field, usePortalRoot);

            var appRead = new AppRuntime(permCheck.App, Log);

            // Get the content-type definition
            var typeDef = appRead.ContentTypes.Get(contentType);
            var fieldDef = typeDef[field];

            // check if this field exists and is actually a file-field or a string (wysiwyg) field
            if (fieldDef == null || !(fieldDef.Type != "Hyperlink" || fieldDef.Type != "String"))
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Requested field '" + field + "' type doesn't allow upload"));
            Log.Add($"field type:{fieldDef.Type}");
            // check for special extensions

            try
            {
                var folder = ContainerContext.Folder();

                if (!string.IsNullOrEmpty(subFolder))
                    folder = ContainerContext.Folder(subFolder, false);
                var filesCollection = HttpContext.Current.Request.Files;
                Log.Add($"folder: {folder}, file⋮{filesCollection.Count}");
                if (filesCollection.Count > 0)
                {
                    var originalFile = filesCollection[0];

                    // start with a security check - so we only upload into valid adam if that's the scenario
                    var dnnFolder = FolderManager.Instance.GetFolder(folder.Id);
                    if (onlyInsideAdam)
                        EnsureItIsInItemOrThrow(guid, field, dnnFolder.PhysicalPath);

                    #region check content-type extensions...

                    // Check file size and extension
                    var fileName = originalFile.FileName;
                    if (!IsAllowedDnnExtension(fileName))
                        throw Http.NotAllowedFileType(fileName, "Not in whitelisted CMS file types.");

                    if (IsKnownRiskyExtension(fileName))
                        throw Http.NotAllowedFileType(fileName, "This is a known risky file type.");

                    // todo: check metadata of the FieldDef to see if still allowed

                    #endregion

                    if (originalFile.ContentLength > (1024 * MaxFileSizeKb))
                        return new UploadResult
                        {
                            Success = false,
                            Error = $"file too large - more than {MaxFileSizeKb}Kb"
                        };

                    // remove forbidden / troubling file name characters
                    fileName = fileName
                        .Replace("%", "per")
                        .Replace("#", "hash");

                    if (fileName != originalFile.FileName)
                        Log.Add($"cleaned file name from'{originalFile.FileName}' to '{fileName}'");


                    // Make sure the image does not exist yet, cycle through numbers (change file name)
                    var numberedFile = fileName;
                    for (var i = 1;
                        FileManager.Instance.FileExists(dnnFolder, Path.GetFileName(numberedFile)) && i < 1000;
                        i++)
                        numberedFile = Path.GetFileNameWithoutExtension(fileName)
                                       + "-" + i + Path.GetExtension(fileName);
                    fileName = numberedFile;

                    // Everything is ok, add file
                    var dnnFile = FileManager.Instance.AddFile(dnnFolder, Path.GetFileName(fileName),
                        originalFile.InputStream);

                    return new UploadResult
                    {
                        Success = true,
                        Error = "",
                        Name = Path.GetFileName(fileName),
                        Id = dnnFile.FileId,
                        Path = PortalSettings.HomeDirectory + dnnFile.RelativePath,
                        Type = Classification.TypeName(dnnFile.Extension)
                    };
                }
                Log.Add("upload one complete");
                return new UploadResult {Success = false, Error = "No image was uploaded."};
            }
            catch (HttpResponseException he)
            {
                return new UploadResult { Success = false, Error = he.Response.ReasonPhrase };
            }
            catch(Exception e)
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
        public IEnumerable<AdamItem> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false)
        {
            Log.Add($"adam items a:{appId}, i:{guid}, field:{field}, subfold:{subfolder}, useRoot:{usePortalRoot}");
            var permCheck = new AppAndPermissions(SxcInstance, appId, Log);

            permCheck.EnsureOrThrow(GrantSets.WriteSomething, contentType);
            var onlyInsideAdam = !UserMayWriteEverywhereOrThrowIfAttempted(usePortalRoot, permCheck.Permissions);
            var app = permCheck.App;
            app.InitData(true, false, new ValueCollectionProvider());

            PrepCore(app, guid, field, usePortalRoot);
            var folderManager = FolderManager.Instance;

            // get root and at the same time auto-create the core folder in case it's missing (important)
            ContainerContext.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            var currentAdam = ContainerContext.Folder(subfolder, false);
            var currentDnn = folderManager.GetFolder(currentAdam.Id);

            if (onlyInsideAdam)
                EnsureItIsInItemOrThrow(guid, field, currentDnn.PhysicalPath);

            var subfolders =  folderManager.GetFolders(currentDnn);
            var files = folderManager.GetFiles(currentDnn);

            var adamFolders =
                subfolders.Where(s => s.FolderID != currentDnn.FolderID)
                    .Select(f => new AdamItem(f) {MetadataId = Metadata.GetMetadataId(AdamAppContext, f.FolderID, true)})
                    .ToList();
            var adamFiles = files
                .Select(f => new AdamItem(f) {MetadataId = Metadata.GetMetadataId(AdamAppContext, f.FileId, false), Type = Classification.TypeName(f.Extension)})
                .ToList();

            var all = adamFolders.Concat(adamFiles).ToList();

            Log.Add($"items complete - will return fld⋮{adamFolders.Count}, files⋮{adamFiles.Count} tot⋮{all.Count}");
            return all;
        }

        [HttpPost]
        public IEnumerable<AdamItem> Folder(int appId, string contentType, Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot)
        {
            Log.Add($"get folders for a:{appId}, i:{guid}, field:{field}, subfld:{subfolder}, new:{newFolder}, useRoot:{usePortalRoot}");
            var permCheck = new AppAndPermissions(SxcInstance, appId, Log);

            permCheck.EnsureOrThrow(GrantSets.WriteSomething, contentType);
            var onlyInsideAdam = !UserMayWriteEverywhereOrThrowIfAttempted(usePortalRoot, permCheck.Permissions);
            PrepCore(permCheck.App, guid, field, usePortalRoot);

            // get root and at the same time auto-create the core folder in case it's missing (important)
            ContainerContext.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            ContainerContext.Folder(subfolder, false);

            // now access the subfolder, creating it if missing (which is what we want
            ContainerContext.Folder(subfolder + "/" + newFolder, true);

            return Items(appId, contentType, guid, field, subfolder, usePortalRoot);
        }

        [HttpGet]
        public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, bool usePortalRoot)
        {
            Log.Add($"delete from a:{appId}, i:{guid}, field:{field}, file:{id}, subf:{subfolder}, isFld:{isFolder}, useRoot:{usePortalRoot}");
            var permCheck = new AppAndPermissions(SxcInstance, appId, Log);
            permCheck.EnsureOrThrow(GrantSets.WriteSomething, contentType);
            var onlyInsideAdam = !UserMayWriteEverywhereOrThrowIfAttempted(usePortalRoot, permCheck.Permissions);
            PrepCore(permCheck.App, guid, field, usePortalRoot);

            // try to see if we can get into the subfolder - will throw error if missing
            var current = ContainerContext.Folder(subfolder, false);
            
            var folderManager = FolderManager.Instance;
            var fileManager = FileManager.Instance;

            if (isFolder)
            {
                var fld = folderManager.GetFolder(id);

                if (onlyInsideAdam)
                    EnsureItIsInItemOrThrow(guid, field, fld.PhysicalPath);

                if (fld.ParentID == current.Id)
                    folderManager.DeleteFolder(id);
                else
                    throw Http.BadRequest("can't delete folder - not found in folder");
            }
            else
            {
                var file = fileManager.GetFile(id);

                if (onlyInsideAdam)
                    EnsureItIsInItemOrThrow(guid, field, file.PhysicalPath);

                if (file.FolderId == current.Id)
                    fileManager.DeleteFile(file);
                else
                    throw Http.BadRequest("can't delete file - not found in folder");
            }

            Log.Add("delete complete");
            return true;
        }

        [HttpGet]
        public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, string newName, bool usePortalRoot)
        {
            Log.Add($"rename a:{appId}, i:{guid}, field:{field}, subf:{subfolder}, isfld:{isFolder}, new:{newName}, useRoot:{usePortalRoot}");

            var permCheck = new AppAndPermissions(SxcInstance, appId, Log);
            permCheck.EnsureOrThrow(GrantSets.WriteSomething, contentType);
            var onlyInsideAdam = !UserMayWriteEverywhereOrThrowIfAttempted(usePortalRoot, permCheck.Permissions);
            PrepCore(permCheck.App, guid, field, usePortalRoot);

            // try to see if we can get into the subfolder - will throw error if missing
            var current = ContainerContext.Folder(subfolder, false);

            var folderManager = FolderManager.Instance;
            var fileManager = FileManager.Instance;

            if (isFolder)
            {
                var fld = folderManager.GetFolder(id);

                if (onlyInsideAdam)
                    EnsureItIsInItemOrThrow(guid, field, fld.PhysicalPath);

                if (fld.ParentID == current.Id)
                    folderManager.RenameFolder(fld, newName);
                else
                    throw Http.BadRequest("can't rename folder - not found in folder");
            }
            else
            {
                // note: never allow to change the extension
                var file = fileManager.GetFile(id);

                if (onlyInsideAdam)
                    EnsureItIsInItemOrThrow(guid, field, file.PhysicalPath);

                if (file.FolderId != current.Id)
                    throw Http.BadRequest("can't rename file - not found in folder");

                if (file.Extension != newName.Split('.').Last())
                    newName += "." + file.Extension;
                fileManager.RenameFile(file, newName);
            }

            Log.Add("rename complete");
            return true;
        }


        #endregion


        #region Security checks


        [AssertionMethod]
        private static void EnsureItIsInItemOrThrow(Guid guid, string field, string path)
        {
            var shortGuid = Mapper.GuidCompress(guid);
            var expectedPathPart = shortGuid + "\\" + field;
            if (path.IndexOf(expectedPathPart, StringComparison.Ordinal) == -1)
                throw new AccessViolationException(
                    "Trying to access a file/folder in a path which is not part of this item - access denied.");
        }


        /// <summary>
        /// Helper to check extension based on DNN settings
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <remarks>
        /// mostly a copy from https://github.com/dnnsoftware/Dnn.Platform/blob/115ae75da6b152f77ad36312eb76327cdc55edd7/DNN%20Platform/Modules/Journal/FileUploadController.cs#L72
        /// </remarks>
        [AssertionMethod]
        private static bool IsAllowedDnnExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return !string.IsNullOrEmpty(extension)
                   && Host.AllowedExtensionWhitelist.IsAllowedExtension(extension.ToLower());
        }

        [AssertionMethod]
        private static bool IsKnownRiskyExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return !string.IsNullOrEmpty(extension)
                   && Security.BadExtensions.IsMatch(extension);
        }

        [AssertionMethod]
        private static bool UserMayWriteEverywhereOrThrowIfAttempted(bool usePortalRoot,
            PermissionCheckBase permCheck)
        {
            var everywhereOk = permCheck.UserMay(GrantSets.WritePublished);
            if (usePortalRoot && !everywhereOk)
                throw Http.BadRequest("you may only create draft-data, so file operations outside of ADAM is not allowed");
            return everywhereOk;
        }


        #endregion

    }
}