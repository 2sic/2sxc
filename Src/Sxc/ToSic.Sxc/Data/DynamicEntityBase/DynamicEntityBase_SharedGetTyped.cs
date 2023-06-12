using System;
using System.Net;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Data
{
    public abstract partial class DynamicEntityBase: ITypedThing
    {
        [PrivateApi]
        IRawHtmlString ITypedThing.Attribute(string name, string noParamOrder = Eav.Parameters.Protector, string attribute = default)
        {
            if (attribute != default)
                return Tag.Attr(attribute, (this as ITypedThing).String(name));

            var value = (this as ITypedThing).String(name);
            return value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
        }

        [PrivateApi]
        dynamic ITypedThing.Dyn => this;


        [PrivateApi]
        DateTime ITypedThing.DateTime(string name, DateTime fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        string ITypedThing.String(string name, string fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        int ITypedThing.Int(string name, int fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        bool ITypedThing.Bool(string name, bool fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        long ITypedThing.Long(string name, long fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        float ITypedThing.Float(string name, float fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        decimal ITypedThing.Decimal(string name, decimal fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        double ITypedThing.Double(string name, double fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        string ITypedThing.Url(string name, string fallback = default)
        {
            var url = Get(name, convertLinks: true) as string;
            return Tags.SafeUrl(url).ToString();
        }

        #region Thing

        //[PrivateApi]
        //ITypedThing Thing(string name, object fallback = default)
        //{
        //    var inner = Get(name, fallback: fallback);
            
        //    if (inner is null) return null;
        //    if (inner is string innerStr)
        //        return DynamicJacket.AsDynamicJacket(innerStr, fallback as string);
        //}

        #endregion
    }
}
