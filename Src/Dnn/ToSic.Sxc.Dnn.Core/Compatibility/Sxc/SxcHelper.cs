using System;
using ToSic.Eav.DataFormats.EavLight;

namespace ToSic.Sxc.Compatibility.Sxc;

/// <summary>
/// This is for compatibility - old code had a Sxc.Serializer.Prepare code which should still work
/// </summary>
[Obsolete]
public class SxcHelper
{
    public SxcHelper(bool editAllowed, IConvertToEavLight innerConverter)
    {
        _editAllowed = editAllowed;
        _innerConverter = innerConverter;
    }
    private readonly bool _editAllowed;
    private readonly IConvertToEavLight _innerConverter;

    public OldDataToDictionaryWrapper Serializer => _entityToDictionary ??= new OldDataToDictionaryWrapper(_editAllowed, _innerConverter);
    private OldDataToDictionaryWrapper _entityToDictionary;
}