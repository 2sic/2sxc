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
    public abstract partial class DynamicJacketBase: ITypedRead
    {
        [PrivateApi]
        IRawHtmlString ITypedRead.Attribute(string name, string noParamOrder, string fallback, string attribute)
        {
            var value = GetV(name, noParamOrder: noParamOrder, fallback: fallback);
            return attribute != default 
                ? Tag.Attr(attribute, value)
                : value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
        }

        TValue ITypedRead.Get<TValue>(string name)
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

        dynamic ITypedRead.Dyn => this;

        bool ITypedRead.Bool(string name, string noParamOrder , bool fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        DateTime ITypedRead.DateTime(string name, string noParamOrder, DateTime fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        string ITypedRead.String(string name, string noParamOrder, string fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        int ITypedRead.Int(string name, string noParamOrder, int fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        long ITypedRead.Long(string name, string noParamOrder, long fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        float ITypedRead.Float(string name, string noParamOrder, float fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        decimal ITypedRead.Decimal(string name, string noParamOrder, decimal fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        double ITypedRead.Double(string name, string noParamOrder, double fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        string ITypedRead.Url(string name, string noParamOrder, string fallback)
        {
            var url =  GetV(name, noParamOrder: noParamOrder, fallback: fallback);
            return Tags.SafeUrl(url).ToString();
        }
    }
}
