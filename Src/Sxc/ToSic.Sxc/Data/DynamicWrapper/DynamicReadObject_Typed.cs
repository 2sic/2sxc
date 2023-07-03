using System;
using System.Net;
using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Data
{
    public partial class DynamicReadObject: ITyped
    {
        dynamic ITyped.Dyn => this;

        bool ITyped.Bool(string name, string noParamOrder, bool fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        string ITyped.String(string name, string noParamOrder, string fallback, bool scrubHtml)
        {
            var value = GetV(name, noParamOrder: noParamOrder, fallback: fallback);
#pragma warning disable CS0618
            return scrubHtml ? Razor.Blade.Tags.Strip(value) : value;
#pragma warning restore CS0618

        }

        int ITyped.Int(string name, string noParamOrder, int fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        long ITyped.Long(string name, string noParamOrder, long fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        float ITyped.Float(string name, string noParamOrder, float fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        double ITyped.Double(string name, string noParamOrder, double fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        string ITyped.Url(string name, string noParamOrder, string fallback)
        {
            var url = GetV(name, noParamOrder: noParamOrder, fallback);
            return Tags.SafeUrl(url).ToString();
        }

        TValue ITyped.Get<TValue>(string name) => GetV<TValue>(name, Eav.Parameters.Protector, default);

        TValue ITyped.Get<TValue>(string name, string noParamOrder, TValue fallback)
        {
            Eav.Parameters.Protect(noParamOrder, $"{nameof(fallback)}");
            return FindValueOrNull(name).ConvertOrFallback(fallback);
        }
        private TValue GetV<TValue>(string name, string noParamOrder, TValue fallback, [CallerMemberName] string cName = default)
        {
            Eav.Parameters.Protect(noParamOrder, $"{nameof(fallback)}", methodName: cName);
            return FindValueOrNull(name).ConvertOrFallback(fallback);
        }

        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback, string attribute)
        {
            var value = GetV(name, noParamOrder: noParamOrder, fallback: fallback);
            return attribute != default
                ? Tag.Attr(attribute, value)
                : value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
        }
    }
}
