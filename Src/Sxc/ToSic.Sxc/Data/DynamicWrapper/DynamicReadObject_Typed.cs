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

        bool ITypedRead.Bool(string name, string noParamOrder, bool fallback) => ((ITypedRead)this).Get(name, noParamOrder: noParamOrder, fallback: fallback);

        DateTime ITypedRead.DateTime(string name, string noParamOrder, DateTime fallback) => ((ITypedRead)this).Get(name, noParamOrder: noParamOrder, fallback: fallback);

        string ITypedRead.String(string name, string noParamOrder, string fallback) => ((ITypedRead)this).Get(name, noParamOrder: noParamOrder, fallback: fallback);

        int ITypedRead.Int(string name, string noParamOrder, int fallback) => ((ITypedRead)this).Get(name, noParamOrder: noParamOrder, fallback: fallback);

        long ITypedRead.Long(string name, string noParamOrder, long fallback) => ((ITypedRead)this).Get(name, noParamOrder: noParamOrder, fallback: fallback);

        float ITypedRead.Float(string name, string noParamOrder, float fallback) => ((ITypedRead)this).Get(name, noParamOrder: noParamOrder, fallback: fallback);

        decimal ITypedRead.Decimal(string name, string noParamOrder, decimal fallback) => ((ITypedRead)this).Get(name, noParamOrder: noParamOrder, fallback: fallback);

        double ITypedRead.Double(string name, string noParamOrder, double fallback) => ((ITypedRead)this).Get(name, noParamOrder: noParamOrder, fallback: fallback);

        string ITypedRead.Url(string name, string noParamOrder, string fallback)
        {
            var url = ((ITypedRead)this).String(name, noParamOrder: noParamOrder, fallback);
            return Tags.SafeUrl(url).ToString();
        }

        TValue ITypedRead.Get<TValue>(string name) => ((ITypedRead)this).Get<TValue>(name);

        TValue ITypedRead.Get<TValue>(string name, string noParamOrder, TValue fallback)
        {
            Eav.Parameters.Protect(noParamOrder, $"{nameof(fallback)}");
            return FindValueOrNull(name).ConvertOrFallback(fallback);
        }

        IRawHtmlString ITypedRead.Attribute(string name, string noParamOrder, string attribute)
        {
            var value = (this as ITypedRead).String(name, noParamOrder: noParamOrder);
            return attribute != default
                ? Tag.Attr(attribute, value)
                : value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
        }
    }
}
