using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Adam.Work.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamWorkGet(AdamWorkBase.MyServices services)
    : AdamWorkBase(services, "Adm.TrnFld")
{
    public AdamFolderFileSet ItemsInField(string subFolderName, bool autoCreate = false)
    {
        var l = Log.Fn<AdamFolderFileSet>($"Subfolder: {subFolderName}; AutoCreate: {autoCreate}");

        l.A("starting permissions checks");
        if (AdamContext.Security.UserIsRestricted && !AdamContext.Security.FieldPermissionOk(GrantSets.ReadSomething))
            return l.ReturnNull("user is restricted, and doesn't have permissions on field - return null");

        // check that if the user should only see drafts, he doesn't see items of published data
        if (!AdamContext.Security.UserIsNotRestrictedOrItemIsDraft(AdamContext.ItemGuid, out _))
            return l.ReturnNull("user is restricted (no read-published rights) and item is published - return null");

        l.A("first permission checks passed");

        // get root and at the same time auto-create the core folder in case it's missing (important)
        var root = AdamContext.AdamRoot.RootFolder(autoCreate);

        // if no root exists then quit now
        if (!autoCreate && root == null)
            return l.Return(new(null, [], []), "no folder");

        // try to see if we can get into the subfolder - will throw error if missing
        var currentFolder = AdamContext.AdamRoot.Folder(subFolderName, false);

        // ensure that it's superuser, or the folder is really part of this item
        if (!AdamContext.Security.SuperUserOrAccessingItemFolder(currentFolder.Path, out var ex))
        {
            l.A("user is not super-user and folder doesn't seem to be an ADAM folder of this item - will throw");
            throw l.Ex(ex);
        }

        var adamFolders = currentFolder.Folders
            //.Cast<Sxc.Adam.Internal.Folder<TFolderId, TFileId>>()
            //.Where(s => !EqualityComparer<TFolderId>.Default.Equals(s.SysId, ((IAssetSysId<TFolderId>)currentFolder).SysId))
            //.Cast<IFolder>()
            .Where(s => !Services.AdamGenericHelper.FoldersHaveSameId(s, currentFolder))
            .ToList();

        // Get/Cast Files
        var adamFiles = currentFolder.Files
            .ToList();

        return l.Return(new(currentFolder, adamFolders, adamFiles), $"ok - fld⋮{adamFolders.Count}, files⋮{adamFiles.Count}");
    }

}