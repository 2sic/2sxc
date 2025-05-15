// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Conversion;

/// <summary>
/// Deprecated since v12, announced for removal in v15, removed in v20.
/// </summary>
[PrivateApi("Made private in v12.04, as it shouldn't be used in razor - but previously it was in some apps so we must assume it's in use")]
[Obsolete]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class EntitiesToDictionary
{
    public EntitiesToDictionary()
        => throw new NotSupportedException(ToSic.Eav.Factory.GenerateMessage("ToSic.Eav.Conversion.EntitiesToDictionary", "https://go.2sxc.org/brc-13-conversion"));
}