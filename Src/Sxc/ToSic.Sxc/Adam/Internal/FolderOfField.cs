namespace ToSic.Sxc.Adam.Internal;

/// <summary>
/// The ADAM Navigator creates a folder object for an entity/field combination
/// This is the root folder where all files for this field are stored
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class FolderOfField<TFolderId, TFileId> : Folder<TFolderId, TFileId>
{
    public FolderOfField(AdamManager<TFolderId, TFileId> adamManager, AdamStorageOfField<TFolderId, TFileId> adamStorageOfField) : base(adamManager)
    {
        if (!AdamManager.Exists(adamStorageOfField.Root))
        {
            // WIP - maybe still provide some basic info?
            //Url = adamStorageOfField.Manager.AdamFs.GetUrl(adamStorageOfField.Root);
            return;
        }

        var f = AdamManager.Folder(adamStorageOfField.Root);
        if (f == null) return;

        Path = f.Path;
        Modified = f.Modified;
        SysId = f.SysId;
        Created = f.Created;
        Modified = f.Modified;

        // IAdamItem interface properties
        Name = f.Name;
        Url = f.Url;

        PhysicalPath = f.PhysicalPath;
    }
}