using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Data.Typed.TypedHelpers;

namespace ToSic.Sxc.Data
{
    internal class CodeItemHelper
    {
        public ITyped Data { get; }
        public readonly GetAndConvertHelper Helper;

        public CodeItemHelper(GetAndConvertHelper helper, ITyped data)
        {
            Data = data;
            Helper = helper;
        }

        #region Keys

        public bool ContainsData(string name)
        {
            var result = Get(name, Protector, required: false);
            if (result == null) return false;
            // edge case: could return an empty list...
            if (result is IEnumerable<ITypedItem> typedList)
                return typedList.Any();
            return true;
        }


        #endregion

        #region Get

        public object Get(string name, string noParamOrder, bool? required, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(required), methodName: cName);
            var findResult = Helper.TryGet(name);
            return IsErrStrict(findResult.Found, required, Helper.StrictGet)
                ? throw ErrStrictForTyped(Data, name, cName: cName)
                : findResult.Result;
        }

        public TValue G4T<TValue>(string name, string noParamOrder, TValue fallback, bool? required = default, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(fallback), methodName: cName);
            var findResult = Helper.TryGet(name);
            return IsErrStrict(findResult.Found, required, Helper.StrictGet)
                ? throw ErrStrictForTyped(Data, name, cName: cName)
                : findResult.Result.ConvertOrFallback(fallback);
        }


        #endregion

        public IRawHtmlString Attribute(string name, string noParamOrder, string fallback, bool? required)
        {
            var result = Get(name, noParamOrder, required);
            var strValue = Helper.Cdf.Services.ForCode.ForCode(result, fallback: fallback);
            return strValue is null ? null : new RawHtmlString(WebUtility.HtmlEncode(strValue));
        }

        public string String(string name, string noParamOrder, string fallback, bool? required, bool scrubHtml)
        {
            var value = G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);
            return scrubHtml ? Helper.Cdf.Services.Scrub.All(value) : value;
        }

        public string Url(string name, string noParamOrder, string fallback, bool? required)
        {
            // TODO: STRICT
            var url = Helper.GetInternal(name, lookupLink: true).Result as string;
            return Tags.SafeUrl(url).ToString();
        }

    }
}
