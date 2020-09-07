using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Web.Http.Controllers;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Adam;
using HttpException = ToSic.Sxc.WebApi.HttpException;

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
        public UploadResultDto Upload(int appId, string contentType, Guid guid, string field, [FromUri] string subFolder = "", bool usePortalRoot = false)
            => UploadOne(appId, contentType, guid, field, subFolder, usePortalRoot);

        private UploadResultDto UploadOne(int appId, string contentType, Guid guid, string field, string subFolder, bool usePortalRoot)
        {
            // wrap all of it in try/catch, to reformat error in better way for js to tell the user
            try
            {
                // Check if the request contains multipart/form-data.
                if (!Request.Content.IsMimeMultipartContent())
                    throw HttpException.NotAllowedFileType("unknown", "doesn't look like a file-upload");


                var filesCollection = HttpContext.Current.Request.Files;
                if (filesCollection.Count > 0)
                {
                    var originalFile = filesCollection[0];
                    var file = new AdamUploader(GetBlock(), appId, Log)
                        .UploadOne(originalFile.InputStream, originalFile.FileName, contentType, guid, field, subFolder, usePortalRoot, false);

                    return new UploadResultDto
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
                return new UploadResultDto { Success = false, Error = "No image was uploaded." };
            }
            catch (HttpResponseException he)
            {
                return new UploadResultDto { Success = false, Error = he.Response.ReasonPhrase };
            }
            catch (Exception e)
            {
                return new UploadResultDto { Success = false, Error = e.Message };
            }
        }

        #region adam-file manager

        // test method to provide a public API for accessing adam items easily
        [HttpGet]
        public IEnumerable<AdamItemDto> ItemsWithAppIdFromContext(string contenttype, Guid guid, string field,
            string folder = "")
        {
            // if app-path specified, use that app, otherwise use from context
            var appId = GetBlock().AppId;
            return Items(appId, contenttype, guid, field, folder);
        }

        [HttpGet]
        public IEnumerable<AdamItemDto> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false)
        {
            var state = new AdamState(GetBlock(), appId, contentType, field, guid, usePortalRoot, Log);
            var callLog = Log.Call<IEnumerable<AdamItemDto>>($"adam items a:{appId}, i:{guid}, field:{field}, subfolder:{subfolder}, useRoot:{usePortalRoot}");
            var results = new AdamBackend().Init(Log).ItemsInField(state, guid, subfolder, usePortalRoot);
            return callLog("ok",  results);
        }

        //private IEnumerable<AdamItemDto> AdamItems(AdamState state, Guid entityGuid, string fieldSubfolder, bool usePortalRoot)
        //{
        //    var wrapLog = Log.Call<IEnumerable<AdamItemDto>>();

        //    Log.Add("starting permissions checks");
        //    if (state.Security.UserIsRestricted && !state.Security.FieldPermissionOk(GrantSets.ReadSomething))
        //        return wrapLog("user is restricted, and doesn't have permissions on field - return null", null);

        //    // check that if the user should only see drafts, he doesn't see items of published data
        //    if (!state.Security.UserIsNotRestrictedOrItemIsDraft(entityGuid, out _))
        //        return wrapLog("user is restricted (no read-published rights) and item is published - return null", null);

        //    Log.Add("first permission checks passed");


        //    // get root and at the same time auto-create the core folder in case it's missing (important)
        //    state.ContainerContext.Folder();

        //    // try to see if we can get into the subfolder - will throw error if missing
        //    var currentAdam = state.ContainerContext.Folder(fieldSubfolder, false);
        //    var fs = state.AdamAppContext.EnvironmentFs;
        //    var currentFolder = fs.GetFolder(currentAdam.Id);

        //    // ensure that it's super user, or the folder is really part of this item
        //    if (!state.Security.SuperUserOrAccessingItemFolder(currentFolder.Path, out var exp))
        //    {
        //        Log.Add("user is not super-user and folder doesn't seem to be an ADAM folder of this item - will throw");
        //        throw exp;
        //    }

        //    var subfolders = currentFolder.Folders.ToList();
        //    var files = currentFolder.Files.ToList();

        //    var dtoMaker = Factory.Resolve<AdamItemDtoMaker>();
        //    var allDtos = new List<AdamItemDto>();

        //    var currentFolderDto = dtoMaker.Create(currentFolder, usePortalRoot, state);
        //    currentFolderDto.Name = ".";
        //    currentFolderDto.MetadataId = currentFolder.Metadata.EntityId;
        //    allDtos.Insert(0, currentFolderDto);

        //    var adamFolders = subfolders.Where(s => s.Id != currentFolder.Id)
        //        .Select(f =>
        //        {
        //            var dto = dtoMaker.Create(f, usePortalRoot, state);
        //            dto.MetadataId = Metadata.GetMetadataId(state.AdamAppContext.AppRuntime, f.Id, true);
        //            return dto;
        //        })
        //        .ToList();
        //    allDtos.AddRange(adamFolders);

        //    var adamFiles = files
        //        .Select(f =>
        //        {
        //            var dto = dtoMaker.Create(f, usePortalRoot, state);
        //            dto.MetadataId = Metadata.GetMetadataId(state.AdamAppContext.AppRuntime, f.Id, false);
        //            dto.Type = Classification.TypeName(f.Extension);
        //            return dto;
        //        })
        //        .ToList();
        //    allDtos.AddRange(adamFiles);

        //    return wrapLog($"ok - fld⋮{adamFolders.Count}, files⋮{adamFiles.Count} tot⋮{allDtos.Count}",  allDtos);
        //}

        [HttpPost]
        public IEnumerable<AdamItemDto> Folder(int appId, string contentType, Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot) 
            => new AdamBackend().Init(Log).Folder(GetBlock(), appId, contentType, guid, field, subfolder, newFolder, usePortalRoot);

        //internal IEnumerable<AdamItemDto> Folder(IBlock block, int appId, string contentType, Guid guid, string field, string subfolder, string newFolder,
        //    bool usePortalRoot)
        //{
        //    Log.Add(
        //        $"get folders for a:{appId}, i:{guid}, field:{field}, subfld:{subfolder}, new:{newFolder}, useRoot:{usePortalRoot}");
        //    var state = new AdamState(block, appId, contentType, field, guid, usePortalRoot, Log);
        //    if (state.Security.UserIsRestricted && !state.Security.FieldPermissionOk(GrantSets.ReadSomething))
        //        return null;

        //    // get root and at the same time auto-create the core folder in case it's missing (important)
        //    var folder = state.ContainerContext.Folder();

        //    // try to see if we can get into the subfolder - will throw error if missing
        //    if (!string.IsNullOrEmpty(subfolder))
        //        folder = state.ContainerContext.Folder(subfolder, false);

        //    // validate that dnn user have write permissions for folder in case dnn file system is used (usePortalRoot)
        //    if (usePortalRoot && !state.Security.CanEditFolder(folder.Id))
        //        throw HttpException.PermissionDenied("can't create new folder - permission denied");

        //    var newFolderPath = string.IsNullOrEmpty(subfolder)
        //        ? newFolder
        //        : Path.Combine(subfolder, newFolder).Replace("\\", "/");

        //    // now access the subfolder, creating it if missing (which is what we want
        //    state.ContainerContext.Folder(newFolderPath, true);

        //    return new AdamBackend().Init(Log)
        //        .ItemsInField(state, guid, subfolder,
        //            usePortalRoot); // Items(appId, contentType, guid, field, subfolder, usePortalRoot);
        //}

        [HttpGet]
        public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, bool usePortalRoot) 
            => new AdamBackend().Init(Log).Delete(GetBlock(), appId, contentType, guid, field, subfolder, isFolder, id, usePortalRoot);

        //private bool Delete(IBlock block, int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id,
        //    bool usePortalRoot)
        //{
        //    Log.Add(
        //        $"delete from a:{appId}, i:{guid}, field:{field}, file:{id}, subf:{subfolder}, isFld:{isFolder}, useRoot:{usePortalRoot}");
        //    var state = new AdamState(block, appId, contentType, field, guid, usePortalRoot, Log);
        //    if (!state.Security.UserIsPermittedOnField(GrantSets.DeleteSomething, out var exp))
        //        throw exp;

        //    // check that if the user should only see drafts, he doesn't see items of published data
        //    if (!state.Security.UserIsNotRestrictedOrItemIsDraft(guid, out var permissionException))
        //        throw permissionException;

        //    // try to see if we can get into the subfolder - will throw error if missing
        //    var current = state.ContainerContext.Folder(subfolder, false);

        //    var fs = state.AdamAppContext.EnvironmentFs;
        //    if (isFolder)
        //    {
        //        var target = fs.GetFolder(id);
        //        // validate that dnn user have write permissions for folder where is file in case dnn file system is used (usePortalRoot)
        //        VerifySecurityAndStructure(state, current, target, id, usePortalRoot, "can't delete folder");
        //        fs.Delete(target);
        //    }
        //    else
        //    {
        //        var target = fs.GetFile(id);
        //        // validate that dnn user have write permissions for folder where is file in case dnn file system is used (usePortalRoot)
        //        VerifySecurityAndStructure(state, current, target, target.FolderId, usePortalRoot, "can't delete file");
        //        fs.Delete(target);
        //    }

        //    Log.Add("delete complete");
        //    return true;
        //}

        //[AssertionMethod]
        //private static void VerifySecurityAndStructure(AdamState state, Eav.Apps.Assets.IAsset parentFolder, Eav.Apps.Assets.IAsset target, int folderId, bool usePortalRoot, string errPrefix)
        //{
        //    // validate that dnn user have write permissions for folder in case dnn file system is used (usePortalRoot)
        //    if (usePortalRoot && !state.Security.CanEditFolder(folderId)) 
        //        throw HttpException.PermissionDenied(errPrefix + " - permission denied");

        //    if (!state.Security.SuperUserOrAccessingItemFolder(target.Path, out var exp))
        //        throw exp;

        //    if (target.ParentId != parentFolder.Id)
        //        throw HttpException.BadRequest(errPrefix + " - not found in folder");
        //}

        [HttpGet]
        public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, string newName, bool usePortalRoot) 
            => new AdamBackend().Init(Log).Rename(GetBlock(), appId, contentType, guid, field, subfolder, isFolder, id, newName, usePortalRoot);

        //private bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id,
        //    string newName, bool usePortalRoot, IBlock block)
        //{
        //    Log.Add(
        //        $"rename a:{appId}, i:{guid}, field:{field}, subf:{subfolder}, isfld:{isFolder}, new:{newName}, useRoot:{usePortalRoot}");

        //    var state = new AdamState(block, appId, contentType, field, guid, usePortalRoot, Log);
        //    if (!state.Security.UserIsPermittedOnField(GrantSets.WriteSomething, out var exp))
        //        throw exp;

        //    // check that if the user should only see drafts, he doesn't see items of published data
        //    if (!state.Security.UserIsNotRestrictedOrItemIsDraft(guid, out var permissionException))
        //        throw permissionException;

        //    // try to see if we can get into the subfolder - will throw error if missing
        //    var current = state.ContainerContext.Folder(subfolder, false);

        //    var fs = state.AdamAppContext.EnvironmentFs;
        //    if (isFolder)
        //    {
        //        var folder = fs.GetFolder(id);

        //        // validate that dnn user have write permissions for folder in case dnn file system is used (usePortalRoot)
        //        if (usePortalRoot && !state.Security.CanEditFolder(folder.Id)) //DnnAdamSecurityChecks.CanEdit(fld))
        //            throw HttpException.PermissionDenied("can't rename folder - permission denied");

        //        if (!state.Security.SuperUserOrAccessingItemFolder(folder.Path, out exp))
        //            throw exp;

        //        if (folder.ParentId != current.Id)
        //            throw HttpException.BadRequest("can't rename folder - not found in folder");

        //        fs.Rename(folder, newName);
        //        //var folderManager = FolderManager.Instance;
        //        //var fld = folderManager.GetFolder(id);
        //        //folderManager.RenameFolder(fld, newName);
        //    }
        //    else
        //    {
        //        // var test = state.ContainerContext.
        //        var adamFile = fs.GetFile(id);

        //        // validate that dnn user have write permissions for folder where is file in case dnn file system is used (usePortalRoot)
        //        if (usePortalRoot && !state.Security.CanEditFolder(adamFile.FolderId)) //DnnAdamSecurityChecks.CanEdit(file))
        //            throw HttpException.PermissionDenied("can't rename file - permission denied");

        //        if (!state.Security.SuperUserOrAccessingItemFolder(adamFile.Path, out exp))
        //            throw exp;

        //        if (adamFile.FolderId != current.Id)
        //            throw HttpException.BadRequest("can't rename file - not found in folder");

        //        // never allow to change the extension
        //        if (adamFile.Extension != newName.Split('.').Last())
        //            newName += "." + adamFile.Extension;
        //        fs.Rename(adamFile, newName);
        //        //var fileManager = FileManager.Instance;
        //        //var file = fileManager.GetFile(id);
        //        //fileManager.RenameFile(file, newName);
        //    }

        //    Log.Add("rename complete");
        //    return true;
        //}

        #endregion
    }
}