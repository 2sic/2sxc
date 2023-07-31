using System;
using System.Net;
using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Services;
using static ToSic.Eav.Parameters;

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

        // 2023-07-31 turned off again as not final and probably not a good idea #ITypedIndexer
        //[PrivateApi]
        //IRawHtmlString ITyped.this[string name] => new TypedItemValue(Get(name));


        TValue ITyped.Get<TValue>(string name) => GetV<TValue>(name, Protector, default);

        TValue ITyped.Get<TValue>(string name, string noParamOrder, TValue fallback)
        {
            Protect(noParamOrder, nameof(fallback));
            return FindValueOrNull(name).ConvertOrFallback(fallback);
        }
        private TValue GetV<TValue>(string name, string noParamOrder, TValue fallback, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(fallback), methodName: cName);
            return FindValueOrNull(name).ConvertOrFallback(fallback);
        }

        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback)
        {
            Protect(noParamOrder, nameof(fallback));
            var value = FindValueOrNull(name);
            var strValue = WrapperFactory.ConvertForCode.ForCode(value, fallback: fallback);
            //ConvertForCodeService.DateForCode(value, out var dateString)
            //    ? dateString
            //    : value.ConvertOrFallback(fallback);
            return strValue is null ? null : new RawHtmlString(WebUtility.HtmlEncode(strValue));
        }
    }
}
