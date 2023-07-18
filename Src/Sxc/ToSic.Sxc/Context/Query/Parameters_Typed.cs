using System;
using System.Net;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context.Query
{
    public partial class Parameters: ITyped
    {
        public dynamic Dyn => this;

        bool ITyped.Bool(string name, string noParamOrder, bool fallback)
            => Get(name, noParamOrder: noParamOrder, fallback: fallback);

        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback)
            => Get(name, noParamOrder: noParamOrder, fallback: fallback);

        string ITyped.String(string name, string noParamOrder, string fallback, bool scrubHtml)
        {
            var value = Get(name, noParamOrder: noParamOrder, fallback: fallback);
#pragma warning disable CS0618
            return scrubHtml ? Razor.Blade.Tags.Strip(value) : value;
#pragma warning restore CS0618
        }

        int ITyped.Int(string name, string noParamOrder, int fallback)
            => Get(name, noParamOrder: noParamOrder, fallback: fallback);

        long ITyped.Long(string name, string noParamOrder, long fallback)
            => Get(name, noParamOrder: noParamOrder, fallback: fallback);

        float ITyped.Float(string name, string noParamOrder, float fallback)
            => Get(name, noParamOrder: noParamOrder, fallback: fallback);

        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback)
            => Get(name, noParamOrder: noParamOrder, fallback: fallback);

        double ITyped.Double(string name, string noParamOrder, double fallback)
            => Get(name, noParamOrder: noParamOrder, fallback: fallback);

        string ITyped.Url(string name, string noParamOrder, string fallback)
        {
            var url = Get(name, noParamOrder: noParamOrder, fallback);
            return Tags.SafeUrl(url).ToString();
        }

        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback)
        {
            var value = Get(name, noParamOrder: noParamOrder, fallback: fallback);
            return value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
        }

        /// <summary>
        /// Note: this is implemented for the sake of the interface, but it won't be used.
        /// Because IParameters has a string this[key] 
        /// </summary>
        [PrivateApi]
        IRawHtmlString ITyped.this[string name] => new TypedItemValue(Get(name));


        object ITyped.Get(string name) => Get(name);
    }
}
