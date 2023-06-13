using System;
using System.Net;
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
        IRawHtmlString ITypedRead.Attribute(string name, string noParamOrder = Eav.Parameters.Protector, string attribute = default)
        {
            if (attribute != default)
                return Tag.Attr(attribute, (this as ITypedRead).String(name));

            var value = (this as ITypedRead).String(name);
            return value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
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
            var result = FindValueOrNull(name, InvariantCultureIgnoreCase, null);
            return result.ConvertOrFallback(fallback);
        }

        dynamic ITypedRead.Dyn => this;

        bool ITypedRead.Bool(string name, bool fallback) => Get(name, fallback: fallback);

        DateTime ITypedRead.DateTime(string name, DateTime fallback) => Get(name, fallback: fallback);

        string ITypedRead.String(string name, string fallback) => Get(name, fallback: fallback);

        int ITypedRead.Int(string name, int fallback) => Get(name, fallback: fallback);

        long ITypedRead.Long(string name, long fallback) => Get(name, fallback: fallback);

        float ITypedRead.Float(string name, float fallback) => Get(name, fallback: fallback);

        decimal ITypedRead.Decimal(string name, decimal fallback) => Get(name, fallback: fallback);

        double ITypedRead.Double(string name, double fallback) => Get(name, fallback: fallback);

        string ITypedRead.Url(string name, string fallback) => Get(name, fallback: fallback);
    }
}
