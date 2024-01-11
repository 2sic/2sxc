using ToSic.Sxc.Data.Internal.Convert;
using ToSic.Sxc.Dnn;
using static ToSic.Eav.Code.Infos.CodeInfoObsolete;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Conversion;

/// <summary>
/// Old implementation with simple constructor. Shouldn't be used any more, because it needs DI now. 
/// </summary>
[PrivateApi("Hide implementation")]
[Obsolete("Please use the new ToSic.Eav.DataFormats.EavLight.IConvertToEavLight service")]
public class DataToDictionary: ConvertToEavLightWithCmsInfo
{

    /// <summary>
    /// Old constructor, for old use cases. Was published in tutorial for a while; not ideal...
    /// </summary>
    [PrivateApi]
    [Obsolete("only keep in case external code was using this in apps ca. 2sxc 11. v12+ should use GetService")]
    public DataToDictionary() : base(DnnStaticDi.StaticBuild<MyServices>())
    {
        DnnStaticDi.CodeInfos.Warn(V13To17(nameof(DataToDictionary), "https://go.2sxc.org/brc-13-conversion"));
    }


    /// <summary>
    /// Old constructor, for old use cases. Was published in tutorial for a while; not ideal...
    /// </summary>
    /// <param name="withEdit">Include editing information in serialized result</param>
    ///// <param name="languages"></param>
    [Obsolete("only keep in case external code was using this in apps ca. 2sxc 11. v12+ should use GetService")]
    public DataToDictionary(bool withEdit) : this() => WithEdit = withEdit;
}