using System;
using System.Collections;
using System.Linq;
using System.Net;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Data
{
    public abstract partial class DynamicEntityBase: ITypedRead
    {
        [PrivateApi]
        IRawHtmlString ITypedRead.Attribute(string name, string noParamOrder = Eav.Parameters.Protector, string attribute = default)
        {
            if (attribute != default)
                return Tag.Attr(attribute, (this as ITypedRead).String(name));

            var value = (this as ITypedRead).String(name);
            return value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
        }

        [PrivateApi]
        dynamic ITypedRead.Dyn => this;


        [PrivateApi]
        DateTime ITypedRead.DateTime(string name, DateTime fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        string ITypedRead.String(string name, string fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        int ITypedRead.Int(string name, int fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        bool ITypedRead.Bool(string name, bool fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        long ITypedRead.Long(string name, long fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        float ITypedRead.Float(string name, float fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        decimal ITypedRead.Decimal(string name, decimal fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        double ITypedRead.Double(string name, double fallback = default) => Get(name, fallback: fallback);

        [PrivateApi]
        string ITypedRead.Url(string name, string fallback = default)
        {
            var url = Get(name, convertLinks: true) as string;
            return Tags.SafeUrl(url).ToString();
        }

        #region TypedRead

        [PrivateApi]
        ITypedRead Read(string name, object fallback = default)
        {
            var inner = Get(name, fallback: fallback);

            if (inner is null) return null;
            if (inner is ITypedRead alreadyTyped)
                return alreadyTyped;
            if (inner is string innerStr)
                return DynamicJacket.AsDynamicJacket(innerStr, fallback as string);
            if (inner is ICanBeEntity)
                return _Services.AsC.AsTyped(inner);
            if (inner is IEnumerable innerEnum)
            {
                var first = innerEnum.Cast<object>().FirstOrDefault();
                if (first == null) return null;
                if (first is ITypedRead t2) return t2;
                if (first is ICanBeEntity) return _Services.AsC.AsTyped(first);
            }
            // todo: case object - rewrap into read
            // todo: use shared conversion code for this
            return null;
        }

        #endregion
    }
}
