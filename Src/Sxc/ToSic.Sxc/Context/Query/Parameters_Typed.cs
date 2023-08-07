using System;
using System.Collections.Generic;
using System.Net;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Typed;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Context.Query
{
    public partial class Parameters: ITyped
    {
        [PrivateApi]
        public dynamic Dyn => this;

        [PrivateApi]
        bool ITyped.ContainsKey(string name) => OriginalsAsDic.ContainsKey(name);

        [PrivateApi]
        IEnumerable<string> ITyped.Keys(string noParamOrder, IEnumerable<string> only)
            => TypedHelpers.FilterKeysIfPossible(noParamOrder, only, OriginalsAsDic?.Keys);


        [PrivateApi]
        object ITyped.Get(string name, string noParamOrder, bool? required)
        {
            Protect(noParamOrder, nameof(required));
            return OriginalsAsDic.TryGetValue(name, out var value) ? value : null;
        }

        [PrivateApi]
        bool ITyped.Bool(string name, string noParamOrder, bool fallback, bool? required)
            => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback, bool? required)
            => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        string ITyped.String(string name, string noParamOrder, string fallback, bool? required, bool scrubHtml)
        {
            var value = GetV(name, noParamOrder: noParamOrder, fallback: fallback);
#pragma warning disable CS0618
            return scrubHtml ? Tags.Strip(value) : value;
#pragma warning restore CS0618
        }

        [PrivateApi]
        int ITyped.Int(string name, string noParamOrder, int fallback, bool? required)
            => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        long ITyped.Long(string name, string noParamOrder, long fallback, bool? required)
            => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        float ITyped.Float(string name, string noParamOrder, float fallback, bool? required)
            => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback, bool? required)
            => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        double ITyped.Double(string name, string noParamOrder, double fallback, bool? required)
            => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        string ITyped.Url(string name, string noParamOrder, string fallback, bool? required)
        {
            var url = GetV(name, noParamOrder: noParamOrder, fallback);
            return Tags.SafeUrl(url).ToString();
        }

        [PrivateApi]
        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback, bool? required)
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

    }
}
