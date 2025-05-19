using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.Backend.Adam;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamWorkFolderCreate<TFolderId, TFileId>(AdamWorkBase<TFolderId, TFileId>.MyServices services)
    : AdamWorkBase<TFolderId, TFileId>(services, "Adm.TrnFld")
{
    public bool Create(string parentSubfolder, string newFolder)
    {
        var l = Log.Fn<bool>($"get folders for subfolder:{parentSubfolder}, new:{newFolder}");
        if (AdamContext.Security.UserIsRestricted && !AdamContext.Security.FieldPermissionOk(GrantSets.ReadSomething))
            return l.ReturnFalse();

        // get root and at the same time auto-create the core folder in case it's missing (important)
        var folder = AdamContextTyped.AdamRoot.Folder();

        // try to see if we can get into the subfolder - will throw error if missing
        if (!string.IsNullOrEmpty(parentSubfolder))
            folder = AdamContextTyped.AdamRoot.Folder(parentSubfolder, false);

        // validate that dnn user have write permissions for folder in case dnn file system is used (usePortalRoot)
        if (AdamContext.UseSiteRoot && !AdamContext.Security.CanEditFolder(folder))
            throw HttpException.PermissionDenied("can't create new folder - permission denied");

        var newFolderPath = string.IsNullOrEmpty(parentSubfolder)
            ? newFolder
            : Path.Combine(parentSubfolder, newFolder).Replace("\\", "/");

        // now access the subfolder, creating it if missing (which is what we want
        AdamContextTyped.AdamRoot.Folder(newFolderPath, true);

        return l.ReturnTrue();
    }
}