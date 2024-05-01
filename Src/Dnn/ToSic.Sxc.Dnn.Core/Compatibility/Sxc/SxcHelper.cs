using ToSic.Eav.DataFormats.EavLight;

namespace ToSic.Sxc.Compatibility.Sxc;

/// <summary>
/// This is for compatibility - old code had a Sxc.Serializer.Prepare code which should still work
/// </summary>
[Obsolete]
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class SxcHelper(bool editAllowed, IConvertToEavLight innerConverter)
{
    public OldDataToDictionaryWrapper Serializer => _entityToDictionary ??= new(editAllowed, innerConverter);
    private OldDataToDictionaryWrapper _entityToDictionary;
}