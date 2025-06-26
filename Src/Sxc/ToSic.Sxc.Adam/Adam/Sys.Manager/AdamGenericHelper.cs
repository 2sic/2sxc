using ToSic.Eav.Apps.Assets.Internal;
using ToSic.Sxc.Adam.Sys.Storage;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam.Sys.Manager;

/// <summary>
/// Helper construct so that correctly typed objects can be generated,
/// without the user (mostly the AdamManager) having to know the exact types.
/// </summary>
public abstract class AdamGenericHelper
{
    public abstract IFolder FolderOfField(AdamManager adamManager, AdamStorageOfField storage, IField? field);

    public abstract bool AssetIsChildOfFolder(IFolder parentFolder, ToSic.Eav.Apps.Assets.IAsset target);

    public abstract bool FoldersHaveSameId(IFolder folder1, IFolder folder2);
}

/// <summary>
/// The generic implementation, which must be registered in Startup with the correct types of the platform.
/// </summary>
/// <typeparam name="TFolderId"></typeparam>
/// <typeparam name="TFileId"></typeparam>
public class AdamGenericHelper<TFolderId, TFileId> : AdamGenericHelper
{
    public override IFolder FolderOfField(AdamManager adamManager, AdamStorageOfField storage, IField? field)
        => FolderOfField<TFolderId, TFileId>.Create(adamManager, storage, field);

    public override bool AssetIsChildOfFolder(IFolder parentFolder, ToSic.Eav.Apps.Assets.IAsset target)
    {
        var folderOwnId = ((IAssetSysId<TFolderId>)parentFolder).SysId;
        var assetParentId = ((IAssetWithParentSysId<TFolderId>)target).ParentSysId;
        return EqualityComparer<TFolderId>.Default.Equals(assetParentId, folderOwnId);
    }

    public override bool FoldersHaveSameId(IFolder folder1, IFolder folder2)
    {
        var folder1Id = ((IAssetSysId<TFolderId>)folder1).SysId;
        var folder2Id = ((IAssetSysId<TFolderId>)folder2).SysId;
        return EqualityComparer<TFolderId>.Default.Equals(folder1Id, folder2Id);
    }
}