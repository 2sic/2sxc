using ToSic.Eav.Apps.Assets.Sys;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Adam.Sys.Storage;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam.Sys;

/// <summary>
/// The ADAM Navigator creates a folder object for an entity/field combination
/// This is the root folder where all files for this field are stored
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class FolderOfField<TFolderId, TFileId> : Folder<TFolderId, TFileId>
{
    private FolderOfField(AdamManager adamManager, IField? field) : base(adamManager)
    {
        Field = field;
    }

    public static FolderOfField<TFolderId, TFileId> Create(AdamManager adamManager, AdamStorageOfField adamStorageOfField, IField? field)
    {
        // WIP - maybe still provide some basic info?
        //Url = adamStorageOfField.Manager.AdamFs.GetUrl(adamStorageOfField.Root);
        var quickInit = !adamManager.AdamFs.FolderExists(adamStorageOfField.Root);
        var f = quickInit
            ? null
            : adamManager.Folder(adamStorageOfField.Root);
        if (f == null)
            quickInit = true;

        if (quickInit)
            return new(adamManager, field)
            {
                ParentSysId = default!,
                SysId = default!,
                Created = default,
                Modified = default,
                Path = null!,
                Name = null!,
                PhysicalPath = null!,
            };

        return new(adamManager, field)
        {
            ParentSysId = default!,
            SysId = ((IAssetSysId<TFolderId>)f!).SysId,
            Created = f.Created,
            Modified = f.Modified,
            Path = f.Path,
            Name = f.Name,
            PhysicalPath = f.PhysicalPath,
            Url = f.Url,
        };
    }
}