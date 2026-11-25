using ToSic.Eav.DataFormats.EavLight;

namespace ToSic.Sxc.Dnn.Razor;

/// <summary>
/// This is for compatibility - old code had a Sxc.Serializer.Prepare code which should still work
/// </summary>
[Obsolete]
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class SxcHelper(bool editAllowed, IConvertToEavLight innerConverter)
{
    public OldDataToDictionaryWrapper Serializer => field ??= new(editAllowed, innerConverter);
}