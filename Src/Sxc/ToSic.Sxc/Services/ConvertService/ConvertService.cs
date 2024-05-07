using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Services.Internal;

// ReSharper disable MethodOverloadWithOptionalParameter

namespace ToSic.Sxc.Services;

[PrivateApi("Hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class ConvertService(
    ConvertValueService cnvSvc,
    LazySvc<ConvertForCodeService> code,
    LazySvc<IJsonService> json)
    : ServiceBase("Sxc.CnvSrv", connect: [cnvSvc, code, json]), IConvertService
{
    public bool OptimizeNumbers => true;

    public bool OptimizeBoolean => true;

    public T To<T>(object value) => value.ConvertOrDefault<T>(numeric: OptimizeNumbers, truthy: OptimizeBoolean);

    public T To<T>(object value, NoParamOrder noParamOrder = default, T fallback = default) => cnvSvc.To(value, noParamOrder, fallback);

    public int ToInt(object value) => cnvSvc.To<int>(value);
    public int ToInt(object value, int fallback = 0) => cnvSvc.To(value, fallback: fallback);

    public Guid ToGuid(object value) => cnvSvc.To<Guid>(value);
    public Guid ToGuid(object value, Guid fallback = default) => cnvSvc.To(value, fallback: fallback);

    public float ToFloat(object value) => cnvSvc.To<float>(value);
    public float ToFloat(object value, float fallback = default) => cnvSvc.To(value, fallback: fallback);

    public decimal ToDecimal(object value) => cnvSvc.To<decimal>(value);
    public decimal ToDecimal(object value, decimal fallback = default) => cnvSvc.To(value, fallback: fallback);

    public double ToDouble(object value) => cnvSvc.To<double>(value);
    public double ToDouble(object value, double fallback = default) => cnvSvc.To(value, fallback: fallback);

    public bool ToBool(object value) => cnvSvc.To<bool>(value);
    public bool ToBool(object value, bool fallback = false) => cnvSvc.To(value, fallback: fallback);
        
    public string ToString(object value) => cnvSvc.To<string>(value);

    public string ToString(object value, string fallback = null, NoParamOrder noParamOrder = default, bool fallbackOnNull = true) 
        => cnvSvc.ToString(value, noParamOrder, fallback, fallbackOnNull);

    public string ForCode(object value) => code.Value.ForCode(value);
    public string ForCode(object value, string fallback = default) => code.Value.ForCode(value, fallback: fallback);
        

    public IJsonService Json => json.Value;

    #region Invisible Converts for backward compatibility

    public int ToInt32(object value) => ToInt(value);

    public float ToSingle(object value) => ToFloat(value);

    #endregion
}