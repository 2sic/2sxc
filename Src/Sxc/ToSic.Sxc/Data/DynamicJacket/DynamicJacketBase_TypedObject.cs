using System;
using System.Net;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using static System.StringComparison;

namespace ToSic.Sxc.Data
{
    public abstract partial class DynamicJacketBase: ITypedObject
    {
        [PrivateApi]
        IRawHtmlString ITypedObject.Attribute(string name, string noParamOrder = Eav.Parameters.Protector, string attribute = default)
        {
            if (attribute != default)
                return Tag.Attr(attribute, (this as ITypedObject).String(name));

            var value = (this as ITypedObject).String(name);
            return value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
        }

        TValue ITypedObject.Get<TValue>(string name)
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

        dynamic ITypedObject.Dyn => this;

        bool ITypedObject.Bool(string name, bool fallback) => Get(name, fallback: fallback);

        DateTime ITypedObject.DateTime(string name, DateTime fallback) => Get(name, fallback: fallback);

        string ITypedObject.String(string name, string fallback) => Get(name, fallback: fallback);

        int ITypedObject.Int(string name, int fallback) => Get(name, fallback: fallback);

        long ITypedObject.Long(string name, long fallback) => Get(name, fallback: fallback);

        float ITypedObject.Float(string name, float fallback) => Get(name, fallback: fallback);

        decimal ITypedObject.Decimal(string name, decimal fallback) => Get(name, fallback: fallback);

        double ITypedObject.Double(string name, double fallback) => Get(name, fallback: fallback);

        string ITypedObject.Url(string name, string fallback) => Get(name, fallback: fallback);
    }
}
