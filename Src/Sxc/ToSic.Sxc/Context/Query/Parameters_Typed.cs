using System;
using System.Net;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context.Query
{
    public partial class Parameters: ITyped
    {
        public dynamic Dyn => this;

        bool ITyped.Bool(string name, string noParamOrder, bool fallback)
            => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback)
            => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        string ITyped.String(string name, string noParamOrder, string fallback, bool scrubHtml)
        {
            var value = GetV(name, noParamOrder: noParamOrder, fallback: fallback);
#pragma warning disable CS0618
            return scrubHtml ? Tags.Strip(value) : value;
#pragma warning restore CS0618
        }

        int ITyped.Int(string name, string noParamOrder, int fallback)
            => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        long ITyped.Long(string name, string noParamOrder, long fallback)
            => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        float ITyped.Float(string name, string noParamOrder, float fallback)
            => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback)
            => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        double ITyped.Double(string name, string noParamOrder, double fallback)
            => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        string ITyped.Url(string name, string noParamOrder, string fallback)
        {
            var url = GetV(name, noParamOrder: noParamOrder, fallback);
            return Tags.SafeUrl(url).ToString();
        }

        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback)
        {
            // Note: we won't do special date processing, since all values in the Parameters are strings
            var value = GetV(name, noParamOrder: noParamOrder, fallback: fallback);
            return value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
        }

        // 2023-07-31 turned off again as not final and probably not a good idea #ITypedIndexer
        ///// <summary>
        ///// Note: this is implemented for the sake of the interface, but it won't be used.
        ///// Because IParameters has a string this[key] 
        ///// </summary>
        //[PrivateApi]
        //IRawHtmlString ITyped.this[string name] => new TypedItemValue(Get(name));


        object ITyped.Get(string name) => Get(name);
    }
}
