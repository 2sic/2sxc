using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
#if NET451
using HtmlString = System.Web.HtmlString;
using IHtmlString = System.Web.IHtmlString;
#else
using HtmlString = Microsoft.AspNetCore.Html.HtmlString;
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif

namespace ToSic.Sxc.Web
{
    [PrivateApi("Hide implementation")]
    public class ConvertService: IConvertService
    {
        public ConvertService(JsonService json)
        {
            Json = json;
        }

        public bool OptimizeNumbers => true;

        public bool OptimizeBoolean => true;

        public bool OptimizeRoundtrip => true;

        public T To<T>(object value) => value.ConvertOrDefault<T>(numeric: OptimizeNumbers, truthy: OptimizeBoolean);

        public T To<T>(object value, T fallback) => value.ConvertOrFallback(fallback, numeric: OptimizeNumbers, truthy: OptimizeBoolean, fallbackOnDefault: false);

        public int ToInt(object value) => To<int>(value);

        public float ToFloat(object value) => To<float>(value);

        public double ToDouble(object value) => To<double>(value);

        public bool ToBool(object value) => To<bool>(value);

        public string ToString(object value) => To<string>(value);

        public IHtmlString ToRaw(object value)
        {
            var str = To<string>(value);
            return new HtmlString(str);
        }

        public IJsonService Json { get; }
    }
}
