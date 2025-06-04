using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Identity;

namespace ToSic.Sxc.Adam.Storage.Internal;

/// <summary>
/// Container of the assets of a field
/// each entity+field combination has its own container for assets
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamStorageOfField(): AdamStorage("Adm.OfFld")
{
    private Guid _entityGuid;
    private string _fieldName;

    public AdamStorageOfField InitItemAndField(Guid entityGuid, string fieldName)
    {
        _entityGuid = entityGuid;
        _fieldName = fieldName;
        return this;
    }


    protected override string GeneratePath(string subFolder)
    {
        var l = Log.Fn<string>(subFolder);
        var result = AdamConstants.ItemFolderMask
            .Replace("[AdamRoot]", Manager.Path)
            .Replace("[Guid22]", _entityGuid.GuidCompress())
            .Replace("[FieldName]", _fieldName)
            .Replace("[SubFolder]", subFolder) // often blank, so it will just be removed
            .Replace("//", "/");
        return l.ReturnAndLog(result);
    }
}