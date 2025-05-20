using ToSic.Eav.Apps.Assets.Internal;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam.Internal;

/// <summary>
/// The ADAM Navigator creates a folder object for an entity/field combination
/// This is the root folder where all files for this field are stored
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class FolderOfField<TFolderId, TFileId> : Folder<TFolderId, TFileId>
{
    public FolderOfField(AdamManager adamManager, AdamStorageOfField adamStorageOfField, IField field) : base(adamManager)
    {
        Field = field;

        // WIP - maybe still provide some basic info?
        //Url = adamStorageOfField.Manager.AdamFs.GetUrl(adamStorageOfField.Root);
        if (!AdamFs.FolderExists(adamStorageOfField.Root))
            return;

        var f = AdamManager.Folder(adamStorageOfField.Root);
        if (f == null)
            return;

        Path = f.Path;
        Modified = f.Modified;
        SysId = ((IAssetSysId<TFolderId>)f).SysId;
        Created = f.Created;
        Modified = f.Modified;

        // IAdamItem interface properties
        Name = f.Name;
        Url = f.Url;

        PhysicalPath = f.PhysicalPath;
    }
}