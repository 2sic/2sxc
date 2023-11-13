using System.Net;
using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data.Typed;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Data.Typed.TypedHelpers;

namespace ToSic.Sxc.Data
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
        public bool IsEmpty(string name, string noParamOrder, bool? isBlank)
        {
            var result = Get(name, noParamOrder, required: false);
            return HasKeysHelper.IsEmpty(result, isBlank);
        }

        public bool IsFilled(string name, string noParamOrder, bool? isBlank)
        {
            var result = Get(name, noParamOrder, required: false);
            return HasKeysHelper.IsNotEmpty(result, isBlank);
        }


        #endregion

        #region Get

        public object Get(string name, string noParamOrder, bool? required, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(required), methodName: cName);
            var findResult = Helper.TryGet(name);
            return IsErrStrict(findResult.Found, required, Helper.PropsRequired)
                ? throw ErrStrictForTyped(Data, name, cName: cName)
                : findResult.Result;
        }

        public TValue G4T<TValue>(string name, string noParamOrder, TValue fallback, bool? required = default, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(fallback), methodName: cName);
            var findResult = Helper.TryGet(name);
            return IsErrStrict(findResult.Found, required, Helper.PropsRequired)
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

        public string String(string name, string noParamOrder, string fallback, bool? required, object scrubHtml = default)
        {
            var value = G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);
            return TypedItemHelpers.MaybeScrub(value, scrubHtml, () => Helper.Cdf.Services.Scrub);
        }


        public string Url(string name, string noParamOrder, string fallback, bool? required)
        {
            // TODO: STRICT
            var url = Helper.GetInternal(name, lookupLink: true).Result as string;
            return Tags.SafeUrl(url).ToString();
        }

    }
}
