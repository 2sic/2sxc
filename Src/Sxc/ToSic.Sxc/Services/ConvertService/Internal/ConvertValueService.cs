using ToSic.Eav.Plumbing;
using ToSic.Lib.Coding;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Services.Internal;

[PrivateApi("Hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ConvertValueService() : ServiceBase("Sxc.CnvSrv")
{
    public bool OptimizeNumbers => true;

    public bool OptimizeBoolean => true;

    public T To<T>(object value) => value.ConvertOrDefault<T>(numeric: OptimizeNumbers, truthy: OptimizeBoolean);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public T To<T>(object value, NoParamOrder noParamOrder = default, T fallback = default)
    {
        //ProtectAgainstMissingParameterNames(noParamOrder, nameof(To), nameof(fallback));
        return value.ConvertOrFallback(fallback, numeric: OptimizeNumbers, truthy: OptimizeBoolean, fallbackOnDefault: false);
    }

    public string ToString(object value, NoParamOrder noParamOrder = default, string fallback = null, bool fallbackOnNull = true)
    {
        //Protect(noParamOrder, $"{nameof(fallback)}, {nameof(fallbackOnNull)}");
        var result = To(value, fallback: fallback);
        return result is null && fallbackOnNull ? fallback: result;
    }
        
}