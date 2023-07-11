using System;
using System.Net;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Data
{
    public abstract partial class DynamicEntityBase: ITyped
    {
        [PrivateApi]
        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback)
        {
            var value = GetV(name, noParamOrder: noParamOrder, fallback: fallback);
            return value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
        }

        [PrivateApi]
        dynamic ITyped.Dyn => this;


        [PrivateApi]
        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        string ITyped.String(string name, string noParamOrder, string fallback, bool scrubHtml)
        {
            var value = GetV(name, noParamOrder: noParamOrder, fallback: fallback);
            return scrubHtml ? _Services.Scrub.All(value) : value;
        }

        [PrivateApi]
        int ITyped.Int(string name, string noParamOrder, int fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        bool ITyped.Bool(string name, string noParamOrder, bool fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        long ITyped.Long(string name, string noParamOrder, long fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        float ITyped.Float(string name, string noParamOrder, float fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        double ITyped.Double(string name, string noParamOrder, double fallback) => GetV(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        string ITyped.Url(string name, string noParamOrder, string fallback)
        {
            var url = Get(name, noParamOrder: noParamOrder, convertLinks: true) as string;
            return Tags.SafeUrl(url).ToString();
        }

        [PrivateApi]
        string ITyped.ToString()
        {
            return "test / debug" + ToString();
        }

        //#region TypedRead

        //[PrivateApi]
        //ITyped Read(string name, object fallback = default)
        //{
        //    var inner = GetV(name, fallback: fallback);

        //    if (inner is null) return null;
        //    if (inner is ITyped alreadyTyped)
        //        return alreadyTyped;
        //    if (inner is string innerStr)
        //        return DynamicJacket.AsDynamicJacket(innerStr, fallback as string);
        //    if (inner is ICanBeEntity)
        //        return _Services.AsC.AsItem(inner);
        //    if (inner is IEnumerable innerEnum)
        //    {
        //        var first = innerEnum.Cast<object>().FirstOrDefault();
        //        if (first == null) return null;
        //        if (first is ITyped t2) return t2;
        //        if (first is ICanBeEntity) return _Services.AsC.AsItem(first);
        //    }
        //    // todo: case object - rewrap into read
        //    // todo: use shared conversion code for this
        //    return null;
        //}

        //#endregion
    }
}
