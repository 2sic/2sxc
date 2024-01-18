using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Identity;

namespace ToSic.Sxc.Adam.Internal;

/// <summary>
/// Container of the assets of a field
/// each entity+field combination has its own container for assets
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AdamStorageOfField<TFolderId, TFileId>: AdamStorage<TFolderId, TFileId>
{
    private Guid _entityGuid;
    private string _fieldName;

    public AdamStorageOfField<TFolderId, TFileId> InitItemAndField(Guid entityGuid, string fieldName)
    {
        _entityGuid = entityGuid;
        _fieldName = fieldName;
        return this;
    }


    protected override string GeneratePath(string subFolder)
    {
        var callLog = Log.Fn<string>(subFolder);
        var result = AdamConstants.ItemFolderMask
            .Replace("[AdamRoot]", Manager.Path)
            .Replace("[Guid22]", Mapper.GuidCompress(_entityGuid))
            .Replace("[FieldName]", _fieldName)
            .Replace("[SubFolder]", subFolder) // often blank, so it will just be removed
            .Replace("//", "/");
        return callLog.ReturnAndLog(result);
    }
}