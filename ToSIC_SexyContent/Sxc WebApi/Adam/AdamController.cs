using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Web.Api;
using System.Web.Http.Controllers;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.WebApi;
using ToSic.SexyContent.WebApi.Adam;
using ToSic.SexyContent.WebApi.Errors;

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
    [ValidateAntiForgeryToken]
    public class AdamController : SxcApiControllerBase
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("Api.Adam");
        }

        [HttpPost]
        [HttpPut]
        public UploadResult Upload(int appId, string contentType, Guid guid, string field, [FromUri] string subFolder = "", bool usePortalRoot = false)
            => UploadOne(appId, contentType, guid, field, subFolder, usePortalRoot);

        private UploadResult UploadOne(int appId, string contentType, Guid guid, string field, string subFolder, bool usePortalRoot)
        {
            // wrap all of it in try/catch, to reformat error in better way for js to tell the user
            try
            {
                // Check if the request contains multipart/form-data.
                if (!Request.Content.IsMimeMultipartContent())
                    throw Http.NotAllowedFileType("unknown", "doesn't look like a file-upload");


                var filesCollection = HttpContext.Current.Request.Files;
                if (filesCollection.Count > 0)
                {
                    var originalFile = filesCollection[0];
                    var file = new AdamUploader(SxcInstance, appId, Log)
                        .UploadOne(originalFile.InputStream, originalFile.FileName, contentType, guid, field, subFolder, usePortalRoot, false);

                    return new UploadResult
                    {
                        Success = true,
                        Error = "",
                        Name = Path.GetFileName(file.FullName),
                        Id = file.Id,
                        Path = file.Url,
                        Type = Classification.TypeName(file.Extension)
                    };
                }

                Log.Add("upload one complete");
                return new UploadResult { Success = false, Error = "No image was uploaded." };
            }
            catch (HttpResponseException he)
            {
                return new UploadResult { Success = false, Error = he.Response.ReasonPhrase };
            }
            catch (Exception e)
            {
                return new UploadResult { Success = false, Error = e.Message };
            }
        }

        #region adam-file manager

        // test method to provide a public API for accessing adam items easily
        [HttpGet]
        public IEnumerable<AdamItem> ItemsWithAppIdFromContext(string contenttype, Guid guid, string field,
            string folder = "")
        {
            // if app-path specified, use that app, otherwise use from context
            var appId = SxcInstance.AppId;
            if (appId == null) throw new Exception("Can't detect app-id, module-context missing in http request");
            return Items(appId.Value, contenttype, guid, field, folder);
        }

        [HttpGet]
        public IEnumerable<AdamItem> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false)
        {
            var wrapLog = Log.Call<IEnumerable<AdamItem>>("Items",
                $"adam items a:{appId}, i:{guid}, field:{field}, subfolder:{subfolder}, useRoot:{usePortalRoot}");
            var state = new AdamSecureState(SxcInstance, appId, contentType, field, guid, usePortalRoot, Log);


            Log.Add("starting permissions checks");
            if (state.UserIsRestricted && !state.FieldPermissionOk(GrantSets.ReadSomething))
                return wrapLog("user is restricted, and doesn't have permissions on field - return null", null);

            // check that if the user should only see drafts, he doesn't see items of published data
            if (!state.UserIsNotRestrictedOrItemIsDraft(guid, out var _))
                return wrapLog("user is restricted (no read-published rights) and item is published - return null", null);

            Log.Add("first permission checks passed");


            // get root and at the same time auto-create the core folder in case it's missing (important)
            state.ContainerContext.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            var currentAdam = state.ContainerContext.Folder(subfolder, false);
            var folderManager = FolderManager.Instance;
            var currentDnn = folderManager.GetFolder(currentAdam.Id);

            // ensure that it's super user, or the folder is really part of this item
            if (!state.SuperUserOrAccessingItemFolder(currentDnn.PhysicalPath, out var exp))
            {
                Log.Add("user is not super-user and folder doesn't seem to be an ADAM folder of this item - will throw");
                throw exp;
            }

            var subfolders = folderManager.GetFolders(currentDnn);
            var files = folderManager.GetFiles(currentDnn);

            var all = new List<AdamItem>();

            // currentFolder is needed to get allowEdit for Adam root folder
            var currentFolder = new AdamItem(currentDnn, usePortalRoot, state)
            {
                Name = ".",
                MetadataId = Metadata.GetMetadataId(state.AdamAppContext.AppRuntime, currentDnn.FolderID, true)
            };
            all.Insert(0, currentFolder);

            var adamFolders = subfolders.Where(s => s.FolderID != currentDnn.FolderID)
                .Select(f => new AdamItem(f, usePortalRoot, state)
                {
                    MetadataId = Metadata.GetMetadataId(state.AdamAppContext.AppRuntime, f.FolderID, true)
                })
                .ToList();
            all.AddRange(adamFolders);

            var adamFiles = files
                .Select(f => new AdamItem(f, usePortalRoot, state)
                {
                    MetadataId = Metadata.GetMetadataId(state.AdamAppContext.AppRuntime, f.FileId, false),
                    Type = Classification.TypeName(f.Extension)
                })
                .ToList();
            all.AddRange(adamFiles);

            Log.Add($"items complete - will return fld⋮{adamFolders.Count}, files⋮{adamFiles.Count} tot⋮{all.Count}");
            return all;
        }

        [HttpPost]
        public IEnumerable<AdamItem> Folder(int appId, string contentType, Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot)
        {
            Log.Add($"get folders for a:{appId}, i:{guid}, field:{field}, subfld:{subfolder}, new:{newFolder}, useRoot:{usePortalRoot}");
            var state = new AdamSecureState(SxcInstance, appId, contentType, field, guid, usePortalRoot, Log);
            if (state.UserIsRestricted && !state.FieldPermissionOk(GrantSets.ReadSomething))
            {
                return null;
            }

            // get root and at the same time auto-create the core folder in case it's missing (important)
            var folder = state.ContainerContext.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            if (!string.IsNullOrEmpty(subfolder))
                folder = state.ContainerContext.Folder(subfolder, false);

            // start with a security check...
            var dnnFolder = FolderManager.Instance.GetFolder(folder.Id);

            // validate that dnn user have write permissions for folder in case dnn file system is used (usePortalRoot)
            if (usePortalRoot && !SecurityChecks.CanEdit(dnnFolder))
                throw Http.PermissionDenied("can't create new folder - permission denied");

            var newFolderPath = string.IsNullOrEmpty(subfolder) ? newFolder : Path.Combine(subfolder, newFolder).Replace("\\", "/"); 

            // now access the subfolder, creating it if missing (which is what we want
            state.ContainerContext.Folder(newFolderPath, true);

            return Items(appId, contentType, guid, field, subfolder, usePortalRoot);
        }

        [HttpGet]
        public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, bool usePortalRoot)
        {
            Log.Add($"delete from a:{appId}, i:{guid}, field:{field}, file:{id}, subf:{subfolder}, isFld:{isFolder}, useRoot:{usePortalRoot}");
            var state = new AdamSecureState(SxcInstance, appId, contentType, field, guid, usePortalRoot, Log);
            if (!state.UserIsPermittedOnField(GrantSets.DeleteSomething, out var exp))
                throw exp;

            // check that if the user should only see drafts, he doesn't see items of published data
            if (!state.UserIsNotRestrictedOrItemIsDraft(guid, out var permissionException))
                throw permissionException;

            // try to see if we can get into the subfolder - will throw error if missing
            var current = state.ContainerContext.Folder(subfolder, false);


            if (isFolder)
            {
                var folderManager = FolderManager.Instance;
                var fld = folderManager.GetFolder(id);

                // validate that dnn user have write permissions for folder in case dnn file system is used (usePortalRoot)
                if (usePortalRoot && !SecurityChecks.CanEdit(fld))
                    throw Http.PermissionDenied("can't delete folder - permission denied");

                if (!state.SuperUserOrAccessingItemFolder(fld.PhysicalPath, out exp))
                    throw exp;

                if (fld.ParentID != current.Id)
                    throw Http.BadRequest("can't delete folder - not found in folder");
                folderManager.DeleteFolder(id);
            }
            else
            {
                var fileManager = FileManager.Instance;
                var file = fileManager.GetFile(id);

                // validate that dnn user have write permissions for folder where is file in case dnn file system is used (usePortalRoot)
                if (usePortalRoot && !SecurityChecks.CanEdit(file))
                    throw Http.PermissionDenied("can't delete file - permission denied");

                if (!state.SuperUserOrAccessingItemFolder(file.PhysicalPath, out exp))
                    throw exp;

                if (file.FolderId != current.Id)
                    throw Http.BadRequest("can't delete file - not found in folder");
                fileManager.DeleteFile(file);
            }

            Log.Add("delete complete");
            return true;
        }

        [HttpGet]
        public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, string newName, bool usePortalRoot)
        {
            Log.Add($"rename a:{appId}, i:{guid}, field:{field}, subf:{subfolder}, isfld:{isFolder}, new:{newName}, useRoot:{usePortalRoot}");

            var state = new AdamSecureState(SxcInstance, appId, contentType, field, guid, usePortalRoot, Log);
            if (!state.UserIsPermittedOnField(GrantSets.WriteSomething, out var exp))
                throw exp;

            // check that if the user should only see drafts, he doesn't see items of published data
            if (!state.UserIsNotRestrictedOrItemIsDraft(guid, out var permissionException))
                throw permissionException;

            // try to see if we can get into the subfolder - will throw error if missing
            var current = state.ContainerContext.Folder(subfolder, false);

            if (isFolder)
            {
                var folderManager = FolderManager.Instance;
                var fld = folderManager.GetFolder(id);

                // validate that dnn user have write permissions for folder in case dnn file system is used (usePortalRoot)
                if (usePortalRoot && !SecurityChecks.CanEdit(fld))
                    throw Http.PermissionDenied("can't rename folder - permission denied");

                if (!state.SuperUserOrAccessingItemFolder(fld.PhysicalPath, out exp))
                    throw exp;

                if (fld.ParentID != current.Id)
                    throw Http.BadRequest("can't rename folder - not found in folder");
                folderManager.RenameFolder(fld, newName);
            }
            else
            {
                var fileManager = FileManager.Instance;
                var file = fileManager.GetFile(id);

                // validate that dnn user have write permissions for folder where is file in case dnn file system is used (usePortalRoot)
                if (usePortalRoot && !SecurityChecks.CanEdit(file))
                    throw Http.PermissionDenied("can't rename file - permission denied");

                if (!state.SuperUserOrAccessingItemFolder(file.PhysicalPath, out exp))
                    throw exp;

                if (file.FolderId != current.Id)
                    throw Http.BadRequest("can't rename file - not found in folder");

                // never allow to change the extension
                if (file.Extension != newName.Split('.').Last())
                    newName += "." + file.Extension;
                fileManager.RenameFile(file, newName);
            }

            Log.Add("rename complete");
            return true;
        }

        #endregion
    }
}