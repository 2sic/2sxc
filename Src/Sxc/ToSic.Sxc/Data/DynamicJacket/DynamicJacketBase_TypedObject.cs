using System;
using System.Net;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using static System.StringComparison;

namespace ToSic.Sxc.Data
{
    public abstract partial class DynamicJacketBase: ITypedThing
    {
        [PrivateApi]
        IRawHtmlString ITypedThing.Attribute(string name, string noParamOrder = Eav.Parameters.Protector, string attribute = default)
        {
            if (attribute != default)
                return Tag.Attr(attribute, (this as ITypedThing).String(name));

            var value = (this as ITypedThing).String(name);
            return value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
        }

        TValue ITypedThing.Get<TValue>(string name)
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

        dynamic ITypedThing.Dyn => this;

        bool ITypedThing.Bool(string name, bool fallback) => Get(name, fallback: fallback);

        DateTime ITypedThing.DateTime(string name, DateTime fallback) => Get(name, fallback: fallback);

        string ITypedThing.String(string name, string fallback) => Get(name, fallback: fallback);

        int ITypedThing.Int(string name, int fallback) => Get(name, fallback: fallback);

        long ITypedThing.Long(string name, long fallback) => Get(name, fallback: fallback);

        float ITypedThing.Float(string name, float fallback) => Get(name, fallback: fallback);

        decimal ITypedThing.Decimal(string name, decimal fallback) => Get(name, fallback: fallback);

        double ITypedThing.Double(string name, double fallback) => Get(name, fallback: fallback);

        string ITypedThing.Url(string name, string fallback) => Get(name, fallback: fallback);
    }
}
