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
        [PrivateApi]
        bool ITyped.Has(string name)
        {
            return false; // must be overriden by implementation
        }

        [PrivateApi]
        object ITyped.Get(string name, string noParamOrder, bool? strict)
        {
            Protect(noParamOrder, nameof(strict));
            var findResult = GetInternal(name, lookup: false);
            return IsErrStrict(findResult.Found, strict, StrictGet)
                ? throw ErrStrict(name)
                : findResult.Result;
        }

        //[PrivateApi]
        //TValue ITyped.Get<TValue>(string name) => G4T<TValue>(name, noParamOrder: Protector, fallback: default);

        [PrivateApi]
        TValue ITyped.Get<TValue>(string name, string noParamOrder, TValue fallback, bool? strict)
            => G4T(name, noParamOrder, fallback: fallback, strict: strict);

        /// <summary>
        /// Get for typed G4T
        /// </summary>
        [PrivateApi]
        protected TValue G4T<TValue>(string name, string noParamOrder, TValue fallback, bool? strict = default, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(fallback), methodName: cName);
            var findResult = GetInternal(name, lookup: false);
            return IsErrStrict(findResult.Found, strict, StrictGet)
                ? throw ErrStrict(name, cName)
                : findResult.Result.ConvertOrFallback(fallback);
        }


        [PrivateApi]
        protected bool IsErrStrict(bool found, bool? strict, bool strictGetDefault) 
            => !found && (strict ?? strictGetDefault);

        [PrivateApi]
        protected static ArgumentException ErrStrict(string name, [CallerMemberName] string cName = default)
        {
            var help = $"Correct the name '{name}', or use strict false is AsItem(...)";
            var msg = cName == "."
                ? $".{name} not found and 'strict' is true, meaning that an error is thrown. {help}"
                : $"{cName}('{name}', ...) not found and 'strict' is true, meaning that an error is thrown. {help}";
            return new ArgumentException(msg, nameof(name));
        }

        [PrivateApi]
        IRawHtmlString ITyped.Attribute(string name, string noParamOrder, string fallback, bool? strict)
        {
            Protect(noParamOrder, nameof(fallback));
            var findResult = GetInternal(name, lookup: false);
            if (IsErrStrict(findResult.Found, strict, StrictGet)) throw ErrStrict(name);
            var strValue = _Services.ForCode.ForCode(findResult.Result, fallback: fallback);
            return strValue is null ? null : new RawHtmlString(WebUtility.HtmlEncode(strValue));
        }

        [PrivateApi]
        dynamic ITyped.Dyn => this;


        [PrivateApi]
        DateTime ITyped.DateTime(string name, string noParamOrder, DateTime fallback, bool? strict)
            => G4T(name, noParamOrder: noParamOrder, fallback: fallback, strict: strict);

        [PrivateApi]
        string ITyped.String(string name, string noParamOrder, string fallback, bool? strict, bool scrubHtml)
        {
            var value = G4T(name, noParamOrder: noParamOrder, fallback: fallback, strict: strict);
            return scrubHtml ? _Services.Scrub.All(value) : value;
        }

        [PrivateApi]
        int ITyped.Int(string name, string noParamOrder, int fallback, bool? strict)
            => G4T(name, noParamOrder: noParamOrder, fallback: fallback, strict: strict);

        [PrivateApi]
        bool ITyped.Bool(string name, string noParamOrder, bool fallback, bool? strict) 
            => G4T(name, noParamOrder: noParamOrder, fallback: fallback, strict: strict);

        [PrivateApi]
        long ITyped.Long(string name, string noParamOrder, long fallback, bool? strict)
            => G4T(name, noParamOrder: noParamOrder, fallback: fallback, strict: strict);

        [PrivateApi]
        float ITyped.Float(string name, string noParamOrder, float fallback, bool? strict)
            => G4T(name, noParamOrder: noParamOrder, fallback: fallback, strict: strict);

        [PrivateApi]
        decimal ITyped.Decimal(string name, string noParamOrder, decimal fallback, bool? strict) => G4T(name, noParamOrder: noParamOrder, fallback: fallback, strict: strict);

        [PrivateApi]
        double ITyped.Double(string name, string noParamOrder, double fallback, bool? strict) => G4T(name, noParamOrder: noParamOrder, fallback: fallback, strict: strict);

        [PrivateApi]
        string ITyped.Url(string name, string noParamOrder, string fallback, bool? strict)
        {
            // TODO: STRICT
            var url = Get(name, noParamOrder: noParamOrder, convertLinks: true) as string;
            return Tags.SafeUrl(url).ToString();
        }

        // 2023-07-31 turned off again as not final and probably not a good idea #ITypedIndexer
        //[PrivateApi]
        //IRawHtmlString ITyped.this[string name] => new TypedItemValue(Get(name));


        [PrivateApi]
        string ITyped.ToString() => "test / debug: " + ToString();
    }
}
