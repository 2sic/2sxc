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
using ToSic.Eav.ValueProvider;
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
   public partial class AdamController: SxcApiControllerBase
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
            Log.Add($"upload one a:{appId}, i:{guid}, field:{field}, subfold:{subFolder}, useRoot:{usePortalRoot}");

            // wrap all of it in try/catch, to reformat error in better way for js to tell the user
            try
            {
                // Check if the request contains multipart/form-data.
                if (!Request.Content.IsMimeMultipartContent())
                    throw Http.NotAllowedFileType("unknown", "doesn't look like a file-upload");

                var state = new AdamSecureState(SxcInstance, appId, contentType, field, guid, usePortalRoot, Log);
                state.ThrowIfRestrictedUserIsntPermitted(GrantSets.WriteSomething);

                var folder = state.ContainerContext.Folder();

                if (!string.IsNullOrEmpty(subFolder))
                    folder = state.ContainerContext.Folder(subFolder, false);
                var filesCollection = HttpContext.Current.Request.Files;
                Log.Add($"folder: {folder}, file⋮{filesCollection.Count}");
                if (filesCollection.Count > 0)
                {
                    var originalFile = filesCollection[0];

                    // start with a security check - so we only upload into valid adam if that's the scenario
                    var dnnFolder = FolderManager.Instance.GetFolder(folder.Id);
                    state.ThrowIfOutsidePermittedFolders(dnnFolder.PhysicalPath);

                    #region check content-type extensions...

                    // Check file size and extension
                    var fileName = originalFile.FileName;
                    state.ThrowIfBadExtension(fileName);

                    // todo: check metadata of the FieldDef to see if still allowed extension
                    // note 2018-04-20 2dm: can't do this yet, because wysiwy doesn't have a setting for allowed file-uploads

                    #endregion

                    if (originalFile.ContentLength > 1024 * MaxFileSizeKb)
                        return new UploadResult { Success = false, Error = $"file too large - more than {MaxFileSizeKb}Kb" };

                    // remove forbidden / troubling file name characters
                    fileName = fileName.Replace("%", "per").Replace("#", "hash");

                    if (fileName != originalFile.FileName)
                        Log.Add($"cleaned file name from'{originalFile.FileName}' to '{fileName}'");

                    // Make sure the image does not exist yet, cycle through numbers (change file name)
                    fileName = RenameFileToNotOverwriteExisting(fileName, dnnFolder);

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

        }

        
        #region adam-file manager

        [HttpGet]
        public IEnumerable<AdamItem> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false)
        {
            Log.Add($"adam items a:{appId}, i:{guid}, field:{field}, subfold:{subfolder}, useRoot:{usePortalRoot}");
            var state = new AdamSecureState(SxcInstance, appId, contentType, field, guid, usePortalRoot, Log);
            if (state.UserIsRestricted && !state.FieldPermissionOk(GrantSets.ReadSomething))
            {
                return null;
            }

            var app = state.App;
            app.InitData(true, false, new ValueCollectionProvider());

            var folderManager = FolderManager.Instance;

            // get root and at the same time auto-create the core folder in case it's missing (important)
            state.ContainerContext.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            var currentAdam = state.ContainerContext.Folder(subfolder, false);
            var currentDnn = folderManager.GetFolder(currentAdam.Id);

            state.ThrowIfOutsidePermittedFolders(currentDnn.PhysicalPath);

            var subfolders = folderManager.GetFolders(currentDnn);
            var files = folderManager.GetFiles(currentDnn);

            var adamFolders = subfolders.Where(s => s.FolderID != currentDnn.FolderID)
                .Select(f => new AdamItem(f)
                {
                    MetadataId = Metadata.GetMetadataId(state.AdamAppContext, f.FolderID, true)
                })
                .ToList();
            var adamFiles = files
                .Select(f => new AdamItem(f)
                {
                    MetadataId = Metadata.GetMetadataId(state.AdamAppContext, f.FileId, false),
                    Type = Classification.TypeName(f.Extension)
                })
                .ToList();

            var all = adamFolders.Concat(adamFiles).ToList();

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
            state.ContainerContext.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            state.ContainerContext.Folder(subfolder, false);

            // now access the subfolder, creating it if missing (which is what we want
            state.ContainerContext.Folder(subfolder + "/" + newFolder, true);

            return Items(appId, contentType, guid, field, subfolder, usePortalRoot);
        }

        [HttpGet]
        public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, bool usePortalRoot)
        {
            Log.Add($"delete from a:{appId}, i:{guid}, field:{field}, file:{id}, subf:{subfolder}, isFld:{isFolder}, useRoot:{usePortalRoot}");
            var state = new AdamSecureState(SxcInstance, appId, contentType, field, guid, usePortalRoot, Log);
            state.ThrowIfRestrictedUserIsntPermitted(GrantSets.DeleteSomething);

            // try to see if we can get into the subfolder - will throw error if missing
            var current = state.ContainerContext.Folder(subfolder, false);
            

            if (isFolder)
            {
                var folderManager = FolderManager.Instance;
                var fld = folderManager.GetFolder(id);

                state.ThrowIfOutsidePermittedFolders(fld.PhysicalPath);

                if (fld.ParentID != current.Id)
                    throw Http.BadRequest("can't delete folder - not found in folder");
                folderManager.DeleteFolder(id);
            }
            else
            {
                var fileManager = FileManager.Instance;
                var file = fileManager.GetFile(id);

                state.ThrowIfOutsidePermittedFolders(file.PhysicalPath);

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
            state.ThrowIfRestrictedUserIsntPermitted(GrantSets.WriteSomething);

            // try to see if we can get into the subfolder - will throw error if missing
            var current = state.ContainerContext.Folder(subfolder, false);

            if (isFolder)
            {
                var folderManager = FolderManager.Instance;
                var fld = folderManager.GetFolder(id);
                state.ThrowIfOutsidePermittedFolders(fld.PhysicalPath);

                if (fld.ParentID != current.Id)
                    throw Http.BadRequest("can't rename folder - not found in folder");
                folderManager.RenameFolder(fld, newName);
            }
            else
            {
                var fileManager = FileManager.Instance;
                var file = fileManager.GetFile(id);
                state.ThrowIfOutsidePermittedFolders(file.PhysicalPath);

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