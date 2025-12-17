using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Services.Sys.ConvertService;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Services;

[PrivateApi("Hide implementation")]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class ConvertService16(
    ConvertValueService cnvSvc,
    LazySvc<ConvertForCodeService> code,
    LazySvc<IJsonService> json)
    : ServiceWithContext("Sxc.CnvSrv", connect: [cnvSvc, code, json]), IConvertService16
{


    [field: AllowNull, MaybeNull]
    private ICodeDataFactory Cdf => field ??= ExCtx.GetCdf();

    #region ToMock() new v21


    public ITypedItem ToMockItem(object data, NoParamOrder npo = default, bool? propsRequired = default)
        => Cdf.AsItem(data, new() { ItemIsStrict = true, UseMock = true})!;

    public T ToMock<T>(object data, NoParamOrder npo = default, bool? propsRequired = default)
        where T : class, ICanWrapData
        => Cdf.AsCustom<T>(source: data, mock: true);

    #endregion

    #region New v17 As conversions - removed again for v21, was never published

    /// <summary>
    /// EXPERIMENTAL
    /// </summary>
    /// <returns></returns>
    [PrivateApi("WIP, don't publish yet")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    T IConvertService16.As<T>(ICanBeEntity source, NoParamOrder npo)
        => Cdf.AsCustom<T>(source: source, npo: npo)!;

    /// <summary>
    /// EXPERIMENTAL
    /// </summary>
    /// <returns></returns>
    [PrivateApi("WIP, don't publish yet")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    IEnumerable<T> IConvertService16.AsList<T>(IEnumerable<ICanBeEntity> source, NoParamOrder npo, bool nullIfNull)
        => Cdf.AsCustomList<T>(source: source, npo: npo, nullIfNull: nullIfNull);

    #endregion

    //public bool OptimizeNumbers => true;

    //public bool OptimizeBoolean => true;


    public T? To<T>(object value, NoParamOrder npo = default, T? fallback = default)
        => cnvSvc.To(value, npo, fallback);

    public int ToInt(object value, NoParamOrder npo = default, int fallback = 0)
        => cnvSvc.To(value, fallback: fallback);

    public Guid ToGuid(object value, NoParamOrder npo = default, Guid fallback = default)
        => cnvSvc.To(value, fallback: fallback);

    public float ToFloat(object value, NoParamOrder npo = default, float fallback = default)
        => cnvSvc.To(value, fallback: fallback);

    public decimal ToDecimal(object value, NoParamOrder npo = default, decimal fallback = default)
        => cnvSvc.To(value, fallback: fallback);

    public double ToDouble(object value, NoParamOrder npo = default, double fallback = default)
        => cnvSvc.To(value, fallback: fallback);

    public bool ToBool(object value, NoParamOrder npo = default, bool fallback = false)
        => cnvSvc.To(value, fallback: fallback);
        

    public string? ToString(object value, NoParamOrder npo = default, string? fallback = default, bool fallbackOnNull = true) 
        => cnvSvc.ToString(value, npo, fallback, fallbackOnNull);

    public string? ForCode(object value, NoParamOrder npo = default, string? fallback = default)
        => code.Value.ForCode(value, npo, fallback);
        

    public IJsonService Json => json.Value;


    #region Invisible Converts for backward compatibility - 2dm removed 2024-01-22 since it's not in the interface and can't be in use - del 2024-Q3

    //public int ToInt32(object value) => ToInt(value);

    //public float ToSingle(object value) => ToFloat(value);

    #endregion
}