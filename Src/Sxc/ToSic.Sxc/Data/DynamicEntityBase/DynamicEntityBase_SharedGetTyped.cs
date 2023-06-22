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
        IRawHtmlString ITypedRead.Attribute(string name, string noParamOrder, string attribute)
        {
            var value = (this as ITypedRead).String(name, noParamOrder: noParamOrder);
            return attribute != default 
                ? Tag.Attr(attribute, value)
                : value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
        }

        [PrivateApi]
        dynamic ITypedRead.Dyn => this;


        [PrivateApi]
        DateTime ITypedRead.DateTime(string name, string noParamOrder, DateTime fallback) => Get(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        string ITypedRead.String(string name, string noParamOrder, string fallback) => Get(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        int ITypedRead.Int(string name, string noParamOrder, int fallback) => Get(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        bool ITypedRead.Bool(string name, string noParamOrder, bool fallback) => Get(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        long ITypedRead.Long(string name, string noParamOrder, long fallback) => Get(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        float ITypedRead.Float(string name, string noParamOrder, float fallback) => Get(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        decimal ITypedRead.Decimal(string name, string noParamOrder, decimal fallback) => Get(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        double ITypedRead.Double(string name, string noParamOrder, double fallback) => Get(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        string ITypedRead.Url(string name, string noParamOrder, string fallback)
        {
            var url = Get(name, noParamOrder: noParamOrder, convertLinks: true) as string;
            return Tags.SafeUrl(url).ToString();
        }

        [PrivateApi]
        string ITypedRead.ToString()
        {
            return "test / debug" + ToString();
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
                return _Services.AsC.AsItem(inner);
            if (inner is IEnumerable innerEnum)
            {
                var first = innerEnum.Cast<object>().FirstOrDefault();
                if (first == null) return null;
                if (first is ITypedRead t2) return t2;
                if (first is ICanBeEntity) return _Services.AsC.AsItem(first);
            }
            // todo: case object - rewrap into read
            // todo: use shared conversion code for this
            return null;
        }

        #endregion
    }
}
