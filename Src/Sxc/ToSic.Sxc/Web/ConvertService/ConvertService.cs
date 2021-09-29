using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
#if NET451
using HtmlString = System.Web.HtmlString;
using IHtmlString = System.Web.IHtmlString;
// ReSharper disable MethodOverloadWithOptionalParameter
#else
using HtmlString = Microsoft.AspNetCore.Html.HtmlString;
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif

namespace ToSic.Sxc.Web
{
    [PrivateApi("Hide implementation")]
    public class ConvertService: IConvertService
    {
        public ConvertService(IJsonService json)
        {
            Json = json;
        }

        public bool OptimizeNumbers => true;

        public bool OptimizeBoolean => true;

        public bool OptimizeRoundtrip => true;

        public T To<T>(object value) => value.ConvertOrDefault<T>(numeric: OptimizeNumbers, truthy: OptimizeBoolean);

        public T To<T>(object value, T fallback) => value.ConvertOrFallback(fallback, numeric: OptimizeNumbers, truthy: OptimizeBoolean, fallbackOnDefault: false);

        public int ToInt(object value) => To<int>(value);
        public int ToInt(object value, string noParamOrder = Eav.Parameters.Protector, int fallback = default) => To(value, fallback);

        public float ToFloat(object value) => To<float>(value);
        public float ToFloat(object value, string noParamOrder = Eav.Parameters.Protector, float fallback = default) => To(value, fallback);

        public double ToDouble(object value) => To<double>(value);
        public double ToDouble(object value, string noParamOrder = Eav.Parameters.Protector, double fallback = default) => To(value, fallback);

        public bool ToBool(object value) => To<bool>(value);
        public bool ToBool(object value, string noParamOrder = Eav.Parameters.Protector, bool fallback = default) => To(value, fallback);

        public string ForCode(object value) => ForCode(value, fallback: default);
        public string ForCode(object value, string noParamOrder = Eav.Parameters.Protector, string fallback = default)
        {
            if (value == null) return null;

            // Pre-check special case of date-time which needs ISO encoding without time zone
            if (value.GetType().UnboxIfNullable() == typeof(DateTime))
            {
                var dt = ((DateTime)value).ToString("O").Substring(0, 23) + "z";
                return dt;
            }

            var result = To(value, fallback);
            if (result is null) return null;

            // If the original value was a boolean, we will do case changing as js expects "true" or "false" and not "True" or "False"
            if (value.GetType().UnboxIfNullable() == typeof(bool)) result = result.ToLowerInvariant();

            return result;
        }

        public IJsonService Json { get; }
    }
}
