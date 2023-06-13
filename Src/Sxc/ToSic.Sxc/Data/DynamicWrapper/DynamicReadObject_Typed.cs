using System;
using System.Net;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Data
{
    public partial class DynamicReadObject: ITypedRead
    {
        dynamic ITypedRead.Dyn => this;

        bool ITypedRead.Bool(string name, bool fallback)
            => FindValueOrNull(name).ConvertOrFallback(fallback);

        DateTime ITypedRead.DateTime(string name, DateTime fallback)
            => FindValueOrNull(name).ConvertOrFallback(fallback);

        string ITypedRead.String(string name, string fallback)
            => FindValueOrNull(name).ConvertOrFallback(fallback);

        int ITypedRead.Int(string name, int fallback)
            => FindValueOrNull(name).ConvertOrFallback(fallback);

        long ITypedRead.Long(string name, long fallback)
            => FindValueOrNull(name).ConvertOrFallback(fallback);

        float ITypedRead.Float(string name, float fallback)
            => FindValueOrNull(name).ConvertOrFallback(fallback);

        decimal ITypedRead.Decimal(string name, decimal fallback)
            => FindValueOrNull(name).ConvertOrFallback(fallback);

        double ITypedRead.Double(string name, double fallback)
            => FindValueOrNull(name).ConvertOrFallback(fallback);

        string ITypedRead.Url(string name, string fallback)
        {
            var url = ((ITypedRead)this).String(name, fallback);
            return Tags.SafeUrl(url).ToString();
        }

        TValue ITypedRead.Get<TValue>(string name)
            => FindValueOrNull(name).ConvertOrDefault<TValue>();

        TValue ITypedRead.Get<TValue>(string name, string noParamOrder, TValue fallback)
            => FindValueOrNull(name).ConvertOrFallback(fallback);

        IRawHtmlString ITypedRead.Attribute(string name, string noParamOrder, string attribute)
        {
            var value = (this as ITypedRead).String(name);
            return attribute != default
                ? Tag.Attr(attribute, value)
                : value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
        }
    }
}
