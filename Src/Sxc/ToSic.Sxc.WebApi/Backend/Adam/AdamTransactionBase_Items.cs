using ToSic.Sxc.Adam.Internal;

namespace ToSic.Sxc.Backend.Adam;

partial class AdamTransactionBase<T, TFolderId, TFileId>
{
    /// <inheritdoc />
    public IList<AdamItemDto> ItemsInField(string subFolderName, bool autoCreate = false)
    {
        var l = Log.Fn<IList<AdamItemDto>>($"Subfolder: {subFolderName}");

        l.A("starting permissions checks");
        if (AdamContext.Security.UserIsRestricted && !AdamContext.Security.FieldPermissionOk(GrantSets.ReadSomething))
            return l.ReturnNull("user is restricted, and doesn't have permissions on field - return null");

        // check that if the user should only see drafts, he doesn't see items of published data
        if (!AdamContext.Security.UserIsNotRestrictedOrItemIsDraft(AdamContext.ItemGuid, out _))
            return l.ReturnNull("user is restricted (no read-published rights) and item is published - return null");

        l.A("first permission checks passed");
            
        // This will contain the list of items
        var list = new List<AdamItemDto>();
            
        // get root and at the same time auto-create the core folder in case it's missing (important)
        var root = AdamContext.AdamRoot.Folder(autoCreate);

        // if no root exists then quit now
        if (!autoCreate && root == null) 
            return l.Return(list, "no folder");

        // try to see if we can get into the subfolder - will throw error if missing
        var currentFolder = AdamContext.AdamRoot.Folder(subFolderName, false);
            
        // ensure that it's super user, or the folder is really part of this item
        if (!AdamContext.Security.SuperUserOrAccessingItemFolder(currentFolder.Path, out var ex))
        {
            l.A("user is not super-user and folder doesn't seem to be an ADAM folder of this item - will throw");
            l.Ex(ex);
            throw ex;
        }

        var subfolders = currentFolder.Folders.ToList();
        var files = currentFolder.Files.ToList();

        var dtoMaker = Services.AdamDtoMaker.New().Init(AdamContext);

        var currentFolderDto = dtoMaker.Create(currentFolder);
        currentFolderDto.Name = ".";
        list.Insert(0, currentFolderDto);

        var adamFolders = subfolders
            .Cast<Folder<TFolderId, TFileId>>()
            .Where(s => !EqualityComparer<TFolderId>.Default.Equals(s.SysId, currentFolder.SysId))
            .Select(f => dtoMaker.Create(f))
            .ToList();
        list.AddRange(adamFolders);

        var adamFiles = files
            .Cast<File<TFolderId, TFileId>>()
            .Select(f => dtoMaker.Create(f))
            .ToList();
        list.AddRange(adamFiles);

        return l.Return(list.ToList(), $"ok - fld⋮{adamFolders.Count}, files⋮{adamFiles.Count} tot⋮{list.Count}");
    }

}