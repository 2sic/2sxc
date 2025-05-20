using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam.Manager;

/// <summary>
/// Helper construct so that correctly typed objects can be generated,
/// without the user (mostly the AdamManager) having to know the exact types.
/// </summary>
public abstract class AdamFactory
{
    public abstract IFolder FolderOfField(AdamManager adamManager, AdamStorageOfField storage, IField field);
}

/// <summary>
/// The generic implementation, which must be registered in Startup with the correct types of the platform.
/// </summary>
/// <typeparam name="TFolderId"></typeparam>
/// <typeparam name="TFileId"></typeparam>
public class AdamFactory<TFolderId, TFileId> : AdamFactory
{
    public override IFolder FolderOfField(AdamManager adamManager, AdamStorageOfField storage, IField field)
        => new FolderOfField<TFolderId, TFileId>(adamManager, storage, field);
}