#pragma warning disable IDE0130
namespace ToSic.Sxc.Conversion;
#pragma warning restore IDE0130

/// <summary>
/// Deprecated since v13, announced for removal in v15, removed in v20.
/// </summary>
[Obsolete]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class DataToDictionary
{
    [Obsolete]
    public DataToDictionary()
        => throw new NotSupportedException(ToSic.Eav.Factory.GenerateMessage("ToSic.Sxc.Conversion.DataToDictionary", "https://go.2sxc.org/brc-13-conversion"));
}