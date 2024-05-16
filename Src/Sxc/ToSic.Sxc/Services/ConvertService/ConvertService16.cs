using ToSic.Lib.DI;
using ToSic.Sxc.Services.Internal;

// 2024-01-22 2dm
// Remove all convert methods which are just missing the optional parameters, to make the API smaller.
// Assume it has no side effects, must watch.
// Remove this note 2024-Q3 (ca. July)

namespace ToSic.Sxc.Services;

[PrivateApi("Hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class ConvertService16(
    ConvertValueService cnvSvc,
    LazySvc<ConvertForCodeService> code,
    LazySvc<IJsonService> json)
    : ServiceForDynamicCode("Sxc.CnvSrv", connect: [cnvSvc, code, json]), IConvertService16
{

    #region New v17 As conversions

    /// <summary>
    /// EXPERIMENTAL
    /// </summary>
    /// <returns></returns>
    [PrivateApi("WIP, don't publish yet")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    T IConvertService16.As<T>(ICanBeEntity source, NoParamOrder protector, bool nullIfNull)
        => _CodeApiSvc.Cdf.AsCustom<T>(source: source, protector: protector, mock: nullIfNull);

    /// <summary>
    /// EXPERIMENTAL
    /// </summary>
    /// <returns></returns>
    [PrivateApi("WIP, don't publish yet")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    IEnumerable<T> IConvertService16.AsList<T>(IEnumerable<ICanBeEntity> source, NoParamOrder protector, bool nullIfNull)
        => _CodeApiSvc.Cdf.AsCustomList<T>(source: source, protector: protector, nullIfNull: nullIfNull);

    #endregion

    //public bool OptimizeNumbers => true;

    //public bool OptimizeBoolean => true;

    //public T To<T>(object value) => value.ConvertOrDefault<T>(numeric: OptimizeNumbers, truthy: OptimizeBoolean);

    public T To<T>(object value, NoParamOrder noParamOrder = default, T fallback = default) => cnvSvc.To(value, noParamOrder, fallback);

    //public int ToInt(object value) => _cnvSvc.To<int>(value);
    public int ToInt(object value, NoParamOrder noParamOrder = default, int fallback = 0) => cnvSvc.To(value, fallback: fallback);

    //public Guid ToGuid(object value) => _cnvSvc.To<Guid>(value);
    public Guid ToGuid(object value, NoParamOrder noParamOrder = default, Guid fallback = default) => cnvSvc.To(value, fallback: fallback);

    //public float ToFloat(object value) => _cnvSvc.To<float>(value);
    public float ToFloat(object value, NoParamOrder noParamOrder = default, float fallback = default) => cnvSvc.To(value, fallback: fallback);

    //public decimal ToDecimal(object value) => _cnvSvc.To<decimal>(value);
    public decimal ToDecimal(object value, NoParamOrder noParamOrder = default, decimal fallback = default) => cnvSvc.To(value, fallback: fallback);

    //public double ToDouble(object value) => _cnvSvc.To<double>(value);
    public double ToDouble(object value, NoParamOrder noParamOrder = default, double fallback = default) => cnvSvc.To(value, fallback: fallback);

    //public bool ToBool(object value) => _cnvSvc.To<bool>(value);
    public bool ToBool(object value, NoParamOrder noParamOrder = default, bool fallback = false) => cnvSvc.To(value, fallback: fallback);
        
    //public string ToString(object value) => _cnvSvc.To<string>(value);

    public string ToString(object value, NoParamOrder noParamOrder = default, string fallback = default, bool fallbackOnNull = true) 
        => cnvSvc.ToString(value, noParamOrder, fallback, fallbackOnNull);

    //public string ForCode(object value) => _code.Value.ForCode(value);
    public string ForCode(object value, NoParamOrder noParamOrder = default, string fallback = default) => code.Value.ForCode(value, noParamOrder, fallback);
        

    public IJsonService Json => json.Value;


    #region Invisible Converts for backward compatibility - 2dm removed 2024-01-22 since it's not in the interface and can't be in use - del 2024-Q3

    //public int ToInt32(object value) => ToInt(value);

    //public float ToSingle(object value) => ToFloat(value);

    #endregion
}