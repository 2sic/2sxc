using System;
using System.Net;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Data
{
    public abstract partial class DynamicEntityBase: ITypedObject
    {
        [PrivateApi]
        IRawHtmlString ITypedObject.Attribute(string name, string noParamOrder = Eav.Parameters.Protector, string attribute = default)
        {
            if (attribute != default)
                return Tag.Attr(attribute, (this as ITypedObject).String(name));

            var value = (this as ITypedObject).String(name);
            return value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
        }

        [PrivateApi]
        dynamic ITypedObject.Dyn => this;


        [PrivateApi]
        DateTime ITypedObject.DateTime(string name, DateTime fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        string ITypedObject.String(string name, string fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        int ITypedObject.Int(string name, int fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        bool ITypedObject.Bool(string name, bool fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        long ITypedObject.Long(string name, long fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        float ITypedObject.Float(string name, float fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        decimal ITypedObject.Decimal(string name, decimal fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        double ITypedObject.Double(string name, double fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        string ITypedObject.Url(string name, string fallback = default)
        {
            var url = Get(name, convertLinks: true) as string;
            return Tags.SafeUrl(url).ToString();
        }
    }
}
