using ToSic.Eav.DataFormats.EavLight;
using ToSic.Sxc.Dnn;
using static ToSic.Eav.Code.Infos.CodeInfoObsolete;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Conversion;

/// <summary>
/// A helper to serialize various combinations of entities, lists of entities etc
/// </summary>
[PrivateApi("Made private in v12.04, as it shouldn't be used in razor - but previously it was in some apps so we must assume it's in use")]
[Obsolete("Marked obsolete in 12.05, because it should be constructed with DI. But we're not sure if it's used in code in the wild")]
public class EntitiesToDictionary: ConvertToEavLight
{
    /// <summary>
    /// Old constructor used in some public apps in razor, so it must remain for DNN implementation
    /// But .net 451 only!
    /// In .net .451 it must also set the formatters to use the right date-time, which isn't necessary in .net core. 
    /// </summary>
    /// <remarks>
    /// has an important side effect, this isn't clear from outside!
    /// </remarks>
    public EntitiesToDictionary(): base(DnnStaticDi.StaticBuild<MyServices>())
    {
        DnnStaticDi.CodeInfos.Warn(V13To17(nameof(EntitiesToDictionary), "https://go.2sxc.org/brc-13-conversion"));
    }
}