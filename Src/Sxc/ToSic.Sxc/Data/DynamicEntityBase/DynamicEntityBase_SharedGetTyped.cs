using System;
using System.Net;
using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public abstract partial class DynamicEntityBase: ITyped
    {
        /// <summary>
        /// Get for typed G4T
        /// </summary>
        [PrivateApi]
        protected TValue G4T<TValue>(string name,
            string noParamOrder = Protector,
            TValue fallback = default,
            [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(fallback), methodName: cName);
            var findResult = GetInternal(name, lookup: false);
            if (!findResult.Found && StrictGet) throw new ArgumentException(ErrStrict(name, cName));
            return findResult.Result.ConvertOrFallback(fallback);
        }

        [PrivateApi]
        protected static string ErrStrict(string name, [CallerMemberName] string cName = default)
        {
            var help = $"Correct the name '{name}', or use strict false is AsItem(...)";
            return cName == "." 
                ? $".{name} not found and 'strict' is true, meaning that an error is thrown. {help}"
                : $"{cName}('{name}', ...) not found and 'strict' is true, meaning that an error is thrown. {help}";
        }

        [PrivateApi]
        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback)
        {
            Protect(noParamOrder, nameof(fallback));
            var findResult = GetInternal(name, lookup: false);
            if (!findResult.Found && StrictGet) throw new ArgumentException(ErrStrict(name));
            var strValue = _Services.ForCode.ForCode(findResult.Result, fallback: fallback);
            return strValue is null ? null : new RawHtmlString(WebUtility.HtmlEncode(strValue));
        }

        [PrivateApi]
        dynamic ITyped.Dyn => this;


        [PrivateApi]
        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback) => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        string ITyped.String(string name, string noParamOrder, string fallback, bool scrubHtml)
        {
            var value = G4T(name, noParamOrder: noParamOrder, fallback: fallback);
            return scrubHtml ? _Services.Scrub.All(value) : value;
        }

        [PrivateApi]
        int ITyped.Int(string name, string noParamOrder, int fallback) => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        bool ITyped.Bool(string name, string noParamOrder, bool fallback) => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        long ITyped.Long(string name, string noParamOrder, long fallback) => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        float ITyped.Float(string name, string noParamOrder, float fallback) => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback) => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        double ITyped.Double(string name, string noParamOrder, double fallback) => G4T(name, noParamOrder: noParamOrder, fallback: fallback);

        [PrivateApi]
        string ITyped.Url(string name, string noParamOrder, string fallback)
        {
            var url = Get(name, noParamOrder: noParamOrder, convertLinks: true) as string;
            return Tags.SafeUrl(url).ToString();
        }

        // 2023-07-31 turned off again as not final and probably not a good idea #ITypedIndexer
        //[PrivateApi]
        //IRawHtmlString ITyped.this[string name] => new TypedItemValue(Get(name));


        [PrivateApi]
        string ITyped.ToString() => "test / debug: " + ToString();

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
