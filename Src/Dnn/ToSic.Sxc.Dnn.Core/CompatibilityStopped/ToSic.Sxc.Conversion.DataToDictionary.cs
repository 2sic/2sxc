// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Conversion;

/// <summary>
/// Old implementation with simple constructor. Shouldn't be used any more, because it needs DI now. 
/// </summary>
[PrivateApi("Hide implementation")]
[Obsolete("Please use the new ToSic.Eav.DataFormats.EavLight.IConvertToEavLight service")]
public class DataToDictionary
{
    /// <summary>
    /// Old constructor, for old use cases. Was published in tutorial for a while; not ideal...
    /// </summary>
    [PrivateApi]
    [Obsolete]
    public DataToDictionary()
        => throw new NotSupportedException(ToSic.Eav.Factory.GenerateMessage("ToSic.Sxc.Conversion.DataToDictionary", "https://go.2sxc.org/brc-13-conversion"));
}