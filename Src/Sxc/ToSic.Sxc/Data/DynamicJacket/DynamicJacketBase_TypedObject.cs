using System;
using System.Net;
using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using static System.StringComparison;

namespace ToSic.Sxc.Data
{
    public abstract partial class DynamicJacketBase: ITyped
    {
        [PrivateApi]
        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback)
        {
            var value = GetV(name, noParamOrder: noParamOrder, fallback: fallback);
            return value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
        }

        TValue ITyped.Get<TValue>(string name)
        {
            var result = FindValueOrNull(name, InvariantCultureIgnoreCase, null);
            return result.ConvertOrDefault<TValue>();
        }
        public TValue Get<TValue>(string name,
            string noParamOrder = Eav.Parameters.Protector,
            TValue fallback = default)
        {
            Eav.Parameters.Protect(noParamOrder, $"{nameof(fallback)}");
            var result = FindValueOrNull(name, InvariantCultureIgnoreCase, null);
            return result.ConvertOrFallback(fallback);
        }
        private TValue GetV<TValue>(string name,
            string noParamOrder = Eav.Parameters.Protector,
            TValue fallback = default,
            [CallerMemberName] string cName = default)
        {
            Eav.Parameters.Protect(noParamOrder, $"{nameof(fallback)}", methodName: cName);
            var result = FindValueOrNull(name, InvariantCultureIgnoreCase, null);
            return result.ConvertOrFallback(fallback);
        }

        dynamic ITyped.Dyn => this;

        bool ITyped.Bool(string name, string noParamOrder , bool fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

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
            var url =  GetV(name, noParamOrder: noParamOrder, fallback: fallback);
            return Tags.SafeUrl(url).ToString();
        }

        [PrivateApi]
        IRawHtmlString ITyped.this[string name] => new TypedItemValue(Get(name));

    }
}
